using System;
using Xunit;

namespace IctBaden.Scripting.Test.CsharpScript;

public sealed class ComplexReturnTests : IDisposable
{
    private readonly ScriptEngine _engine;

    // ReSharper disable once NotAccessedField.Local
    private int _errorLine;
    private int _errorColumn;
    private string _errorMessage = string.Empty;

    public ComplexReturnTests()
    {
        _engine = ScriptFactory
            .CreateCsharpScriptEngine([ "IctBaden.Scripting.Test" ]);
        
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
    public void EvalShouldReturnComplexReturnType()
    {
        const string script = """
                              new IctBaden.Scripting.Test.CsharpScript.ComplexReturnType() 
                              {
                                Number = 5,
                                Value = 123.45,
                                Text = "evaluated"
                              } 
                              """;
        var result = _engine.Eval<ComplexReturnType>(script);
        
        Assert.Equal(string.Empty, _errorMessage);
        Assert.NotNull(result);
        
        Assert.Equal(5, result.Number);
        Assert.Equal(123.45, result.Value);
        Assert.Equal("evaluated", result.Text);
    }

    [Fact]
    public void EvalShouldReturnComplexContextReturnType()
    {
        var context = new ScriptContext();
        const string script = """
                              Result = new IctBaden.Scripting.Test.CsharpScript.ComplexReturnType() 
                              {
                                Number = 5,
                                Value = 123.45,
                                Text = "evaluated"
                              } 
                              """;
        var result = _engine.Eval<ComplexReturnType>(script, context);
        
        Assert.Equal(string.Empty, _errorMessage);
        Assert.NotNull(result);
        
        var contextResult = context.Result;
        Assert.Equal(5, contextResult.Number);
        Assert.Equal(123.45, contextResult.Value);
        Assert.Equal("evaluated", contextResult.Text);
    }


}