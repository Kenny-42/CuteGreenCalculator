using System.Globalization;

namespace CuteGreenCalculator;

/// <summary>
/// Core 4-function calculator state machine. Deliberately has no dependency
/// on WPF (or any UI framework) so it can be unit-tested in isolation and
/// reused unchanged if the UI layer ever changes.
///
/// Semantics match an everyday calculator (like Windows Calculator's basic
/// mode): operators apply left-to-right with no precedence, and pressing "="
/// again with no operator in between repeats the last operation.
///
/// The single source of truth is <see cref="_text"/>: the whole editable
/// expression as typed (e.g. "12+34"), not just the last-entered number. All
/// mutation ultimately goes through <see cref="TryInsert"/> (caret-aware
/// insertion, used by both button clicks and typed/pasted characters) or
/// <see cref="SetText"/> (a full resync after an edit the UI already applied
/// natively, e.g. Backspace/Delete/Cut). The older append-style methods
/// (<see cref="InputDigit"/>, <see cref="InputOperator"/>, etc. - see
/// add-calculator-engine) are kept as thin wrappers over
/// <see cref="TryInsert"/> that always insert at the end, preserving their
/// original observable behavior for existing callers/tests.
/// </summary>
public class CalculatorEngine
{
    private const string AllowedCharacters = "0123456789.+-*/";

    private string _text = "0";

    // True right after a fresh start (construction, Clear, or a completed
    // Equals) - see TryInsert's "wholeReplace" case. Sets the tone for
    // whether the *next* digit/decimal/sign typed starts a brand new number
    // (replacing the whole display, e.g. the speed-dial-right-after-equals
    // behavior from add-speed-dial-buttons) or continues appending.
    private bool _startNewEntry = true;

    // Supports "press = again to repeat the last operation".
    private char? _lastOperator;
    private double? _lastOperand;

    public bool IsError { get; private set; }

    /// <summary>The string to show on the calculator's screen.</summary>
    public string Display => IsError ? "Error" : _text;

    public static bool IsAllowedChar(char c) => AllowedCharacters.IndexOf(c) >= 0;

    /// <summary>
    /// Inserts <paramref name="insertText"/> (already-typed or pasted text,
    /// filtered to allowed calculator characters) into the expression at
    /// <paramref name="selectionStart"/>, replacing
    /// <paramref name="selectionLength"/> existing characters first - the
    /// same shape as a WPF TextBox's SelectionStart/SelectionLength. This is
    /// the one path every caret-aware edit (typed characters, pasted text,
    /// and button clicks alike) goes through, so "click to position the
    /// cursor, then insert" and "click a digit button" behave identically.
    ///
    /// Rejects the edit (returning false, leaving state unchanged) if
    /// the engine is in an error state, if nothing allowed survives
    /// filtering, or if the resulting text would not be a legal in-progress
    /// expression (e.g. two operators in a row, a second decimal point
    /// within one number). On success, returns the caret index the UI
    /// should move to.
    /// </summary>
    public bool TryInsert(int selectionStart, int selectionLength, string insertText, out int newCaretIndex)
    {
        newCaretIndex = selectionStart;
        if (IsError) return false;

        insertText = new string(insertText.Where(IsAllowedChar).ToArray());
        if (insertText.Length == 0) return false;

        // A fresh entry (the initial "0", or right after Equals) is replaced
        // wholesale by a new number rather than appended to - but a binary
        // operator instead continues the chain from the existing value
        // (e.g. "5 =" then "+" should give "5+", not replace "5"). '-' is
        // ambiguous (it can be the start of a negative number OR the
        // subtraction operator), so it only gets whole-replace treatment
        // when there's no real value to chain from yet (_text == "0" and,
        // if we just landed on "0" via Equals, that Equals had no prior
        // operation to repeat either) - otherwise, like +/*//, it continues
        // the chain (e.g. "7 =" then "-" should give "7-", not replace "7"
        // with a bare "-", which silently corrupted the next subtraction -
        // see fix-subtract-from-result).
        bool wholeReplace = insertText[0] switch
        {
            '+' or '*' or '/' => false,
            '-' => _text == "0" && !(_startNewEntry && _lastOperator is not null),
            _ => _text == "0" || _startNewEntry,
        };

        string candidate;
        if (wholeReplace)
        {
            candidate = insertText;
        }
        else
        {
            selectionStart = Math.Clamp(selectionStart, 0, _text.Length);
            selectionLength = Math.Clamp(selectionLength, 0, _text.Length - selectionStart);
            candidate = _text.Remove(selectionStart, selectionLength).Insert(selectionStart, insertText);
        }

        if (!IsValidPartialExpression(candidate)) return false;

        _text = candidate;
        _startNewEntry = false;
        newCaretIndex = (wholeReplace ? 0 : selectionStart) + insertText.Length;
        return true;
    }

    /// <summary>
    /// Resyncs the engine's state from text the UI already applied directly
    /// (native Backspace/Delete/Cut/Undo in the display TextBox). Returns
    /// false - meaning the caller should revert the display back to
    /// <see cref="Display"/> - if the edit is rejected (in an error state,
    /// or the result isn't a legal in-progress expression).
    /// </summary>
    public bool SetText(string text)
    {
        if (IsError) return false;
        if (text.Length == 0) text = "0";
        if (!IsValidPartialExpression(text)) return false;

        _text = text;
        _startNewEntry = false;
        return true;
    }

    public void InputDigit(char digit)
    {
        if (!char.IsAsciiDigit(digit)) return;
        TryInsert(_text.Length, 0, digit.ToString(), out _);
    }

    public void InputDecimalPoint() => TryInsert(_text.Length, 0, ".", out _);

    public void InputOperator(char op)
    {
        if (op is not ('+' or '-' or '*' or '/')) return;
        TryInsert(_text.Length, 0, op.ToString(), out _);
    }

    public void Equals()
    {
        if (IsError) return;

        if (!TryTokenize(_text, out var numbers, out var operators))
        {
            SetError();
            return;
        }

        double result;
        char usedOperator;
        double usedOperand;

        if (operators.Count == 0)
        {
            // No operator typed since the last equals - repeat the last
            // completed operation, if there was one (matches an everyday
            // calculator's "press = again" behavior).
            if (_lastOperator is not { } repeatOp) return;
            usedOperand = _lastOperand ?? 0;
            if (!TryApply(repeatOp, numbers[0], usedOperand, out result))
            {
                SetError();
                return;
            }
            usedOperator = repeatOp;
        }
        else
        {
            result = numbers[0];
            for (var i = 0; i < operators.Count; i++)
            {
                if (!TryApply(operators[i], result, numbers[i + 1], out result))
                {
                    SetError();
                    return;
                }
            }
            usedOperator = operators[^1];
            usedOperand = numbers[^1];
        }

        _lastOperator = usedOperator;
        _lastOperand = usedOperand;
        _text = FormatNumber(result);
        _startNewEntry = true;
    }

    /// <summary>Full reset (the "C" button).</summary>
    public void Clear()
    {
        _text = "0";
        _startNewEntry = true;
        _lastOperator = null;
        _lastOperand = null;
        IsError = false;
    }

    /// <summary>
    /// Resets only the trailing operand being entered (the "CE" button); any
    /// earlier chained operation survives (e.g. "5+39" becomes "5+"). Also
    /// clears an error state, since there is nothing meaningful to preserve
    /// while erroring.
    /// </summary>
    public void ClearEntry()
    {
        IsError = false;
        var start = FindLastOperandStart(_text);
        var head = _text[..start];
        _text = head.Length == 0 ? "0" : head;
    }

    /// <summary>Toggles the sign of the trailing operand, in place.</summary>
    public void ToggleSign()
    {
        if (IsError) return;

        var start = FindLastOperandStart(_text);
        var operand = _text[start..];
        if (operand.Length == 0) return;
        if (TryParseNumber(operand, out var value) && value == 0) return;

        var toggled = operand.StartsWith('-') ? operand[1..] : "-" + operand;
        _text = _text[..start] + toggled;
    }

    /// <summary>Replaces the trailing operand with its square root, in place.</summary>
    public void SquareRoot()
    {
        if (IsError) return;

        var start = FindLastOperandStart(_text);
        var operand = _text[start..];
        if (!TryParseNumber(operand, out var value))
        {
            SetError();
            return;
        }
        if (value < 0)
        {
            SetError();
            return;
        }

        _text = _text[..start] + FormatNumber(Math.Sqrt(value));
        _startNewEntry = true;
    }

    /// <summary>
    /// Removes the last character of the expression (the Backspace key).
    /// Resets to "0" once only a single character - or a lone minus sign -
    /// would remain, rather than leaving an invalid/empty entry.
    /// </summary>
    public void Backspace()
    {
        if (IsError) return;

        if (_text.Length <= 1)
        {
            _text = "0";
            return;
        }

        var trimmed = _text[..^1];
        _text = trimmed.Length == 0 || trimmed == "-" ? "0" : trimmed;
        _startNewEntry = false;
    }

    /// <summary>
    /// Accepts an externally-provided numeric string (e.g. pasted from the
    /// clipboard via the legacy whole-value paste path) as the new
    /// expression. Anything that doesn't parse as a finite number is
    /// silently ignored, leaving the display unchanged.
    /// </summary>
    public void PasteValue(string text)
    {
        if (IsError) return;

        if (!double.TryParse(text.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var value)
            || double.IsNaN(value) || double.IsInfinity(value))
        {
            return;
        }

        _text = FormatNumber(value);
        _startNewEntry = false;
    }

    /// <summary>
    /// Finds the index within <paramref name="text"/> where the last operand
    /// (the number after the last binary operator, or the whole text if
    /// there is none) begins. A '-' only counts as a binary operator when it
    /// doesn't immediately start a number segment (i.e. it's not a sign).
    /// </summary>
    private static int FindLastOperandStart(string text)
    {
        var numberStart = true;
        var lastSplit = 0;
        for (var i = 0; i < text.Length; i++)
        {
            var c = text[i];
            var isOperatorHere = c is '+' or '*' or '/' || (c == '-' && !numberStart);
            if (isOperatorHere)
            {
                lastSplit = i + 1;
                numberStart = true;
            }
            else
            {
                numberStart = false;
            }
        }
        return lastSplit;
    }

    /// <summary>
    /// A lightweight character-shape grammar check for a possibly-incomplete
    /// (mid-typing) expression: digits, at most one decimal point per number
    /// segment, and operators that only ever follow a number segment that
    /// has already started (with '-' additionally allowed as a leading sign
    /// at the very start or right after another operator). Does not require
    /// the expression to be numerically complete - "5+", "5.", and "-" are
    /// all valid *partial* expressions - see <see cref="TryTokenize"/> for
    /// the stricter check used when actually evaluating.
    /// </summary>
    private static bool IsValidPartialExpression(string text)
    {
        if (text.Length == 0) return false;

        var state = SegmentState.Empty;
        foreach (var c in text)
        {
            if (char.IsAsciiDigit(c))
            {
                state = state == SegmentState.HasDot ? SegmentState.HasDot : SegmentState.HasDigits;
            }
            else if (c == '.')
            {
                if (state == SegmentState.HasDot) return false;
                state = SegmentState.HasDot;
            }
            else if (c == '-')
            {
                if (state == SegmentState.Empty) state = SegmentState.SignOnly;
                else if (state is SegmentState.HasDigits or SegmentState.HasDot) state = SegmentState.Empty;
                else return false; // a second sign in a row ("--")
            }
            else if (c is '+' or '*' or '/')
            {
                if (state is SegmentState.HasDigits or SegmentState.HasDot) state = SegmentState.Empty;
                else return false; // an operator with no operand before it yet
            }
            else
            {
                return false; // not an allowed calculator character at all
            }
        }
        return true;
    }

    private enum SegmentState { Empty, SignOnly, HasDigits, HasDot }

    /// <summary>
    /// Splits a *complete* expression into operands and the binary operators
    /// between them, evaluating each operand as it's produced. Fails (and
    /// leaves the out parameters empty) if any segment doesn't parse as a
    /// finite number - including a dangling trailing operator/sign, which
    /// <see cref="IsValidPartialExpression"/> otherwise allows mid-typing.
    /// </summary>
    private static bool TryTokenize(string text, out List<double> numbers, out List<char> operators)
    {
        numbers = new List<double>();
        operators = new List<char>();

        var numberStart = true;
        var segmentStart = 0;
        for (var i = 0; i < text.Length; i++)
        {
            var c = text[i];
            var isOperatorHere = c is '+' or '*' or '/' || (c == '-' && !numberStart);
            if (isOperatorHere)
            {
                if (!TryParseNumber(text[segmentStart..i], out var number)) return false;
                numbers.Add(number);
                operators.Add(c);
                segmentStart = i + 1;
                numberStart = true;
            }
            else
            {
                numberStart = false;
            }
        }

        if (!TryParseNumber(text[segmentStart..], out var last)) return false;
        numbers.Add(last);
        return true;
    }

    private static bool TryParseNumber(string text, out double value)
    {
        if (double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value)
            && !double.IsNaN(value) && !double.IsInfinity(value))
        {
            return true;
        }
        value = 0;
        return false;
    }

    private static bool TryApply(char op, double left, double right, out double result)
    {
        switch (op)
        {
            case '+': result = left + right; return true;
            case '-': result = left - right; return true;
            case '*': result = left * right; return true;
            case '/':
                if (right == 0) { result = 0; return false; }
                result = left / right;
                return true;
            default:
                result = 0;
                return false;
        }
    }

    private void SetError()
    {
        IsError = true;
        _text = "0";
        _startNewEntry = true;
        _lastOperator = null;
        _lastOperand = null;
    }

    private static string FormatNumber(double value) =>
        value.ToString("G15", CultureInfo.InvariantCulture);
}
