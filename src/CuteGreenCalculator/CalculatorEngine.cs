using System.Globalization;

namespace CuteGreenCalculator;

/// <summary>
/// Core 4-function calculator state machine. Deliberately has no dependency
/// on WPF (or any UI framework) so it can be unit-tested in isolation and
/// reused unchanged if the UI layer ever changes.
///
/// Semantics match an everyday calculator (like Windows Calculator's basic
/// mode): operators apply immediately left-to-right with no precedence, and
/// pressing "=" again with no operator in between repeats the last operation.
/// </summary>
public class CalculatorEngine
{
    private string _currentEntry = "0";
    private double? _accumulator;
    private char? _pendingOperator;
    private bool _startNewEntry = true;

    // Supports "press = again to repeat the last operation".
    private char? _lastOperator;
    private double? _lastOperand;

    public bool IsError { get; private set; }

    /// <summary>The string to show on the calculator's screen.</summary>
    public string Display => IsError ? "Error" : _currentEntry;

    public void InputDigit(char digit)
    {
        if (IsError || !char.IsAsciiDigit(digit)) return;

        if (_startNewEntry)
        {
            _currentEntry = digit == '0' ? "0" : digit.ToString();
            _startNewEntry = false;
        }
        else if (_currentEntry == "0")
        {
            _currentEntry = digit.ToString();
        }
        else
        {
            _currentEntry += digit;
        }
    }

    public void InputDecimalPoint()
    {
        if (IsError) return;

        if (_startNewEntry)
        {
            _currentEntry = "0.";
            _startNewEntry = false;
            return;
        }

        if (!_currentEntry.Contains('.'))
        {
            _currentEntry += ".";
        }
    }

    public void InputOperator(char op)
    {
        if (IsError) return;

        // Chain: if there's already a pending operator and the user hasn't
        // started a fresh entry, evaluate it first (5 + 3 + 2 => (5+3)+2).
        if (_pendingOperator is { } pending && !_startNewEntry)
        {
            if (!TryApply(pending, _accumulator ?? 0, CurrentValue(), out var result))
            {
                SetError();
                return;
            }
            _accumulator = result;
            _currentEntry = FormatNumber(result);
        }
        else
        {
            _accumulator = CurrentValue();
        }

        _pendingOperator = op;
        _startNewEntry = true;
        _lastOperator = null;
        _lastOperand = null;
    }

    public void Equals()
    {
        if (IsError) return;

        char? op = _pendingOperator ?? _lastOperator;
        if (op is null) return;

        double left = _pendingOperator is not null ? (_accumulator ?? 0) : CurrentValue();
        double right = _pendingOperator is not null ? CurrentValue() : (_lastOperand ?? 0);

        if (!TryApply(op.Value, left, right, out var result))
        {
            SetError();
            return;
        }

        _lastOperator = op;
        _lastOperand = right;

        _currentEntry = FormatNumber(result);
        _accumulator = null;
        _pendingOperator = null;
        _startNewEntry = true;
    }

    public void ToggleSign()
    {
        if (IsError) return;
        _currentEntry = CurrentValue() == 0 ? _currentEntry : FormatNumber(-CurrentValue());
    }

    public void SquareRoot()
    {
        if (IsError) return;

        var value = CurrentValue();
        if (value < 0)
        {
            SetError();
            return;
        }

        _currentEntry = FormatNumber(Math.Sqrt(value));
        _startNewEntry = true;
    }

    /// <summary>Full reset (the "C" button).</summary>
    public void Clear()
    {
        _currentEntry = "0";
        _accumulator = null;
        _pendingOperator = null;
        _startNewEntry = true;
        _lastOperator = null;
        _lastOperand = null;
        IsError = false;
    }

    /// <summary>
    /// Resets only the value being entered (the "CE" button). Any pending
    /// chained operation survives. Also clears an error state, since there
    /// is no meaningful "current entry" to preserve while erroring.
    /// </summary>
    public void ClearEntry()
    {
        _currentEntry = "0";
        _startNewEntry = true;
        IsError = false;
    }

    /// <summary>
    /// Removes the last character of the current entry (the Backspace key).
    /// Resets to "0" once only a single character - or a lone minus sign -
    /// would remain, rather than leaving an invalid/empty entry.
    /// </summary>
    public void Backspace()
    {
        if (IsError || _startNewEntry) return;

        if (_currentEntry.Length <= 1)
        {
            _currentEntry = "0";
            return;
        }

        var trimmed = _currentEntry[..^1];
        _currentEntry = trimmed == "-" ? "0" : trimmed;
    }

    /// <summary>
    /// Accepts an externally-provided numeric string (e.g. pasted from the
    /// clipboard) as the new current entry. Anything that doesn't parse as a
    /// finite number is silently ignored, leaving the display unchanged.
    /// </summary>
    public void PasteValue(string text)
    {
        if (IsError) return;

        if (!double.TryParse(text.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var value)
            || double.IsNaN(value) || double.IsInfinity(value))
        {
            return;
        }

        _currentEntry = FormatNumber(value);
        _startNewEntry = false;
    }

    private double CurrentValue() =>
        double.TryParse(_currentEntry, NumberStyles.Float, CultureInfo.InvariantCulture, out var value)
            ? value
            : 0;

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
        _currentEntry = "0";
        _accumulator = null;
        _pendingOperator = null;
        _startNewEntry = true;
        _lastOperator = null;
        _lastOperand = null;
    }

    private static string FormatNumber(double value) =>
        value.ToString("G15", CultureInfo.InvariantCulture);
}
