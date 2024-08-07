﻿using System;
using Xunit;

namespace IctBaden.Scripting.Test.CsharpScript;

public sealed class TextReplacementTests : IDisposable
{
    private readonly ScriptEngine _engine;
    private bool _disposed;

    public TextReplacementTests()
    {
        _engine = ScriptFactory.CreateCsharpScriptEngine();
        _engine.ScriptError += (line, column, message) => Console.WriteLine($"({line},{column}): {message})");
    }

    [Fact]
    public void ContextValuesBeResolved()
    {
        var context = new ScriptContext
        {
            ["TargetName"] = "Frank"
        };

        const string text = "Hallo, ich bin {{GetValue(\"TargetName\")}} !";
        var result = _engine.ReplaceExpressions(text, context);
        Assert.Equal("Hallo, ich bin Frank !", result);
    }

    [Fact]
    public void MultilineTextShouldBePossible()
    {
        var context = new ScriptContext
        {
            ["TargetName"] = "Frank"
        };

        const string text = "Hallo,\r\n ich bin {{GetValue(\"TargetName\")}} ! \r\n Ja.";
        var result = _engine.ReplaceExpressions(text, context);
        Assert.Equal("Hallo,\r\n ich bin Frank ! \r\n Ja.", result);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _engine.Dispose();
    }

}