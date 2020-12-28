﻿using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace IctBaden.Scripting
{
    public abstract class ScriptEngine : IDisposable
    {
        public string LastError { get; protected set; }

        public abstract T Eval<T>(string expression, object context = null);

        /// <summary>
        /// User error handling
        /// Arguments: line, column, message
        /// </summary>
        public event Action<int, int, string> ScriptError;
        protected virtual bool OnScriptError(int line, int column, string message)
        {
            Trace.TraceError("ScriptError: " + message);
            if (ScriptError == null) return false;

            ScriptError.Invoke(line, column, message);
            return true;
        }

        // ReSharper disable once UnusedMember.Global
        protected ScriptEngine()
        {
            LastError = string.Empty;
        }

        public void Dispose()
        {
        }

        public string ReplaceExpressions(string text, object context = null)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            var exprFinder = new Regex(@"\{\{([^\}]+)\}\}");
            for (; ; )
            {
                var found = exprFinder.Match(text);
                if (!found.Success)
                    break;
                var expression = found.Groups[1].Value;
                var result = Eval<string>(expression, context);
                text = text.Replace(found.Groups[0].Value, result);
            }
            return text;
        }

    }
}
