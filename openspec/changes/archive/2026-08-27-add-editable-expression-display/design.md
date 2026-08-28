## Context

The display previously showed only the operand currently being typed (a
`TextBlock`); operators applied silently and were never rendered. Issue #17
asks for the whole expression to be visible and directly editable - click
anywhere, insert/delete mid-string, select and copy - which means the engine
needs a different model of "what's on screen."

## Decision: the display is the whole expression string

Confirmed with the user: the display shows the full running expression
(`12+34`), not just the last operand. `CalculatorEngine._text` becomes that
single source of truth. This eliminates the old `_accumulator`/
`_pendingOperator` bookkeeping entirely - evaluating `=` just tokenizes
`_text` into operands/operators and reduces left-to-right, which is
mathematically identical to the old eager-chaining approach for a
no-precedence grammar, so `ChainedOperations_EvaluateLeftToRightImmediately`
and friends still pass unchanged.

One piece of state survives outside the text: `_lastOperator`/`_lastOperand`,
needed for "press `=` again with no new operator" to repeat the last
operation (there's nothing to re-derive that from once the text is just a
bare result).

## Decision: one insertion path for everything

`TryInsert(selectionStart, selectionLength, insertText, out newCaretIndex)`
is used by typed characters, pasted text, and every digit/operator/decimal/
speed-dial button - a button click is modeled as "insert this text at the
display's current caret/selection," exactly like typing it. This means
clicking a digit button now edits wherever the user last placed the cursor,
not always the end - a deliberate, desired side effect of making the display
real text.

Validity is a lightweight character-shape state machine (`Empty` /
`SignOnly` / `HasDigits` / `HasDot`) that accepts *incomplete* expressions
mid-typing (`5+`, `5.`, `-`) but rejects the actual malformed cases (two
operators in a row, a second decimal point in one number, a bad character).
Evaluating `=` uses a stricter tokenizer that requires every segment to
parse as a complete number, so a dangling operator becomes an error only at
that point - not while still typing.

## Decision: a "fresh entry" flag, not caret position, drives the legacy "typing after = replaces" convenience

The pre-existing speed-dial spec requires that pressing a digit right after
`=` replaces the shown result rather than appending to it (`2+3=` then `180`
shows `180`, not `5180`). Caret position alone can't distinguish that from
"finished typing a number, keep appending" (both leave the caret at the
end), so `_startNewEntry` is set on construction/`Clear`/a completed `Equals`
and cleared by any successful edit. It only gates *digit-starting* inserts
(not operators, so `5=` then `+` still continues the chain as `5+`) and only
whole-text replacement - a caret deliberately repositioned mid-expression by
the user is a normal splice, since that's the entire point of this issue.

## Decision: legacy API kept, not removed

`InputDigit`/`InputDecimalPoint`/`InputOperator`/`Backspace`/`PasteValue`
remain as thin "insert/replace at the end" wrappers over
`TryInsert`/`SetText` rather than being deleted, so the existing 18 xunit
tests keep exercising real behavior unchanged (per the issue's explicit
requirement) while `CalculatorView` exclusively uses the new caret-aware
surface.

## Decision: native TextBox editing for structural operations, manual control for character insertion

Digits/operators/`.`/`@` are fully hand-rolled through `PreviewTextInput`
(every character is filtered and, if accepted, applied via `TryInsert` with
`e.Handled = true` - never left to the TextBox's own default insertion) so
button clicks and typed keys go through identical logic. Backspace, Delete,
Cut, and drag-selection are left to the TextBox's native behavior (simpler,
and matches ordinary text-field expectations) with a `TextChanged` handler
resyncing the engine via `SetText`, reverting the TextBox's text if the
result somehow isn't a legal expression. This is also why the Delete key
stops doubling as the `CE` shortcut - a real editable field needs Delete to
delete, and `CE` stays reachable via its own button.

## Decision: font auto-shrink is a plain code-behind measurement

`FormattedText` measures the current text at decreasing font sizes against
the output screen's fixed width until it fits (floor at a minimum readable
size). No converter/behavior framework - it's a small, direct
`RefreshDisplay`-time calculation, consistent with this codebase's
preference for plain code-behind over MVVM machinery.
