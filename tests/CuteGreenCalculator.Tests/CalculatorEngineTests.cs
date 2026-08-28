namespace CuteGreenCalculator.Tests;

public class CalculatorEngineTests
{
    private static CalculatorEngine EnterNumber(CalculatorEngine engine, string number)
    {
        foreach (var ch in number)
        {
            if (ch == '.') engine.InputDecimalPoint();
            else engine.InputDigit(ch);
        }
        return engine;
    }

    [Fact]
    public void InitialDisplay_IsZero()
    {
        var engine = new CalculatorEngine();
        Assert.Equal("0", engine.Display);
    }

    [Fact]
    public void SimpleAddition()
    {
        var engine = new CalculatorEngine();
        EnterNumber(engine, "12");
        engine.InputOperator('+');
        EnterNumber(engine, "8");
        engine.Equals();
        Assert.Equal("20", engine.Display);
    }

    [Theory]
    [InlineData('+', 3)]
    [InlineData('-', -1)]
    [InlineData('*', 2)]
    public void BasicOperators(char op, double expected)
    {
        var engine = new CalculatorEngine();
        EnterNumber(engine, "1");
        engine.InputOperator(op);
        EnterNumber(engine, "2");
        engine.Equals();
        Assert.Equal(expected.ToString("G15"), engine.Display);
    }

    [Fact]
    public void ChainedOperations_EvaluateLeftToRightImmediately()
    {
        var engine = new CalculatorEngine();
        EnterNumber(engine, "5");
        engine.InputOperator('+');
        EnterNumber(engine, "3");
        engine.InputOperator('+');
        EnterNumber(engine, "2");
        engine.Equals();
        Assert.Equal("10", engine.Display);
    }

    [Fact]
    public void RepeatingEquals_RepeatsLastOperation()
    {
        var engine = new CalculatorEngine();
        EnterNumber(engine, "4");
        engine.InputOperator('+');
        EnterNumber(engine, "6");
        engine.Equals();
        Assert.Equal("10", engine.Display);

        engine.Equals();
        Assert.Equal("16", engine.Display);
    }

    [Fact]
    public void Clear_FullyResetsPendingOperation()
    {
        var engine = new CalculatorEngine();
        EnterNumber(engine, "5");
        engine.InputOperator('+');
        EnterNumber(engine, "3");
        engine.Clear();
        Assert.Equal("0", engine.Display);

        EnterNumber(engine, "7");
        engine.Equals();
        Assert.Equal("7", engine.Display);
    }

    [Fact]
    public void ClearEntry_PreservesPendingOperation()
    {
        var engine = new CalculatorEngine();
        EnterNumber(engine, "5");
        engine.InputOperator('+');
        EnterNumber(engine, "39");
        engine.ClearEntry();
        EnterNumber(engine, "3");
        engine.Equals();
        Assert.Equal("8", engine.Display);
    }

    [Fact]
    public void LeadingZeros_AreCollapsed()
    {
        var engine = new CalculatorEngine();
        engine.InputDigit('0');
        engine.InputDigit('0');
        engine.InputDigit('5');
        Assert.Equal("5", engine.Display);
    }

    [Fact]
    public void SecondDecimalPoint_IsIgnored()
    {
        var engine = new CalculatorEngine();
        EnterNumber(engine, "1.2");
        engine.InputDecimalPoint();
        engine.InputDigit('5');
        Assert.Equal("1.25", engine.Display);
    }

    [Fact]
    public void ToggleSign_FlipsCurrentValue()
    {
        var engine = new CalculatorEngine();
        EnterNumber(engine, "5");
        engine.ToggleSign();
        Assert.Equal("-5", engine.Display);
        engine.ToggleSign();
        Assert.Equal("5", engine.Display);
    }

    [Fact]
    public void ToggleSign_OnZero_StaysZero()
    {
        var engine = new CalculatorEngine();
        engine.ToggleSign();
        Assert.Equal("0", engine.Display);
    }

    [Fact]
    public void SquareRoot_OfPerfectSquare()
    {
        var engine = new CalculatorEngine();
        EnterNumber(engine, "9");
        engine.SquareRoot();
        Assert.Equal("3", engine.Display);
    }

    [Fact]
    public void SquareRoot_OfNegative_SetsError()
    {
        var engine = new CalculatorEngine();
        EnterNumber(engine, "4");
        engine.ToggleSign();
        engine.SquareRoot();
        Assert.Equal("Error", engine.Display);
        Assert.True(engine.IsError);
    }

    [Fact]
    public void DivideByZero_SetsError()
    {
        var engine = new CalculatorEngine();
        EnterNumber(engine, "5");
        engine.InputOperator('/');
        EnterNumber(engine, "0");
        engine.Equals();
        Assert.Equal("Error", engine.Display);
        Assert.True(engine.IsError);
    }

    [Fact]
    public void WhileInError_OnlyClearIsAccepted()
    {
        var engine = new CalculatorEngine();
        EnterNumber(engine, "5");
        engine.InputOperator('/');
        EnterNumber(engine, "0");
        engine.Equals();

        engine.InputDigit('7');
        engine.InputOperator('+');
        engine.Equals();
        Assert.Equal("Error", engine.Display);

        engine.Clear();
        Assert.False(engine.IsError);
        Assert.Equal("0", engine.Display);

        engine.InputDigit('7');
        Assert.Equal("7", engine.Display);
    }

    [Fact]
    public void Backspace_RemovesLastCharacter()
    {
        var engine = new CalculatorEngine();
        EnterNumber(engine, "123");
        engine.Backspace();
        Assert.Equal("12", engine.Display);
    }

    [Fact]
    public void Backspace_OnSingleDigit_ResetsToZero()
    {
        var engine = new CalculatorEngine();
        EnterNumber(engine, "7");
        engine.Backspace();
        Assert.Equal("0", engine.Display);
    }

    [Fact]
    public void Backspace_NeverLeavesABareMinusSign()
    {
        var engine = new CalculatorEngine();
        EnterNumber(engine, "5");
        engine.ToggleSign();
        Assert.Equal("-5", engine.Display);

        engine.Backspace();
        Assert.Equal("0", engine.Display);
    }

    [Fact]
    public void Backspace_OnFreshEntry_IsNoOp()
    {
        var engine = new CalculatorEngine();
        engine.Backspace();
        Assert.Equal("0", engine.Display);
    }

    [Fact]
    public void PasteValue_ValidNumber_ReplacesCurrentEntry()
    {
        var engine = new CalculatorEngine();
        engine.PasteValue("3.5");
        Assert.Equal("3.5", engine.Display);

        engine.InputDigit('1');
        Assert.Equal("3.51", engine.Display);
    }

    [Fact]
    public void PasteValue_InvalidText_IsIgnored()
    {
        var engine = new CalculatorEngine();
        EnterNumber(engine, "42");
        engine.PasteValue("hello");
        Assert.Equal("42", engine.Display);
    }

    [Fact]
    public void PasteValue_WhileInError_IsIgnored()
    {
        var engine = new CalculatorEngine();
        EnterNumber(engine, "5");
        engine.InputOperator('/');
        EnterNumber(engine, "0");
        engine.Equals();
        Assert.Equal("Error", engine.Display);

        engine.PasteValue("9");
        Assert.Equal("Error", engine.Display);
    }

    [Fact]
    public void FloatingPointNoise_IsAvoided()
    {
        var engine = new CalculatorEngine();
        EnterNumber(engine, "0.1");
        engine.InputOperator('+');
        EnterNumber(engine, "0.2");
        engine.Equals();
        Assert.Equal("0.3", engine.Display);
    }
}
