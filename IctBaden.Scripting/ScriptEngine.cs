using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace IctBaden.Scripting;

public abstract class ScriptEngine : IDisposable
{
    private Regex? _expressionFinder;
    public string LastError { get; protected set; } = string.Empty;

    public abstract T Eval<T>(string expression, object? context = null);

    /// <summary>
    /// User error handling
    /// Arguments: line, column, message
    /// </summary>
    public event Action<int, int, string>? ScriptError;
    protected virtual bool OnScriptError(int line, int column, string message)
    {
        Trace.TraceError("ScriptError: " + message);
        if (ScriptError == null) return false;

        ScriptError.Invoke(line, column, message);
        return true;
    }

    public virtual void Dispose()
    {
    }

    public string ReplaceExpressions(string text, object? context = null)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        _expressionFinder ??= new Regex(@"\{\{([^\}]+)\}\}", RegexOptions.Compiled);
        while(true)
        {
            var found = _expressionFinder.Match(text);
            if (!found.Success)
                break;
            var expression = found.Groups[1].Value;
            var result = Eval<string>(expression, context);
            text = text.Replace(found.Groups[0].Value, result);
        }
        return text;
    }

}