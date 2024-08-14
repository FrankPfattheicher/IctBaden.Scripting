using System;
using System.Collections.Generic;
using Xunit;

namespace IctBaden.Scripting.Test.CsharpScript;

public sealed class ExpressionTests : IDisposable
{
    private readonly ScriptEngine _engine;

    // ReSharper disable once NotAccessedField.Local
    private int _errorLine;
    private int _errorColumn;
    private string _errorMessage = string.Empty;

    public ExpressionTests()
    {
        _engine = ScriptFactory.CreateCsharpScriptEngine();
        _engine.ScriptError += _engine_ScriptError;
    }

    private void _engine_ScriptError(int line, int column, string message)
    {
        _errorLine = line;
        _errorColumn = column;
        _errorMessage = message;
    }

    public void Dispose()
    {
        _engine.Dispose();
    }

    [Fact]
    public void OneAndOneShouldBeTwo()
    {
        const string script = "1 + 1";
        var result = _engine.Eval<int>(script);
        Assert.Equal(2, result);
    }

    [Fact]
    public void IncompleteExpressionShouldRiseError()
    {
        const string script = "1 +";
        var result = _engine.Eval<string>(script);
        Assert.Equal(string.Empty, result);
        Assert.Equal(4, _errorColumn);
        Assert.Contains("CS1733", _errorMessage);
    }

    [Fact]
    public void MachineNameShouldBeResolved()
    {
        const string script = "Environment.MachineName";
        var result = _engine.Eval<string>(script);
        Assert.Equal(Environment.MachineName, result);
    }

    [Fact]
    public void ContextDictionaryValuesBeResolved()
    {
        var context = new ScriptContext
        {
            ["Var1"] = 1000,
            ["Var2"] = 234
        };

        const string script = "(int)GetValue(\"Var1\") + (int)GetValue(\"Var2\")";
        var result = _engine.Eval<int>(script, context);
        Assert.Equal(1234, result);
    }

    [Fact]
    public void ContextDynamicValuesBeResolved()
    {
        var context = new ScriptContext
        {
            Var =
            {
                Value1 = 1000,
                Value2 = 234
            }
        };

        const string script = "Var.Value1 + Var.Value2";
        var result = _engine.Eval<int>(script, context);
        Assert.Equal(1234, result);
    }

    [Fact]
    public void ContextDictionaryValuesShouldBeDirectlyResolved()
    {
        const string expected = "1000234";
        var context = new ScriptContext
        {
            ["Var1"] = 1000,
            ["Var2"] = "234"
        };

        const string script = "Var1.ToString() + Var2";
        var result = _engine.Eval<string>(script, context);
        Assert.Equal(expected, result);
    }
    
    [Fact]
    public void EvalShouldReturnChangedContextValues()
    {
        const int expected = 345;
        var context = new ScriptContext
        {
            Var =
            {
                Value1 = 111,
                Value2 = 222
            },
            ["Var1"] = 1000,
            ["Var2"] = 345
        };

        const string script = """
                              Var.Value1 = Var2;
                              Var1 = Var2
                              """;
        var result = _engine.Eval<int>(script, context);
        Assert.Equal(expected, result);
        
        var byName = (IDictionary<string,object>)context.Var;
        Assert.Equal(expected, byName["Value1"]);
    }

    [Fact]
    public void SettingContextValuesShouldBeUsedInExpression()
    {
        const int expected = 345;
        var context = new ScriptContext();
        
        context.SetVar("Value1", 111);
        context.SetVar("Value2", 222);
        context["Var1"] = 1000;
        context["Var2"] = 345;

        const string script = """
                              Var.Value1 = Var2;
                              Var1 = Var2
                              """;
        var result = _engine.Eval<int>(script, context);
        Assert.Equal(expected, result);

        var value1 = context.GetVar("Value1");
        Assert.Equal(expected, value1);

        var value2 = context.GetVar("Value2");
        Assert.Equal(222, value2);
    }

}