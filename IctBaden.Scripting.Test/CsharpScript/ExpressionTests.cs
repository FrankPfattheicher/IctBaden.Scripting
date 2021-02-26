using System;
using Xunit;

namespace IctBaden.Scripting.Test.CsharpScript
{
    public class ExpressionTests : IDisposable
    {
        private readonly ScriptEngine _engine;
        // ReSharper disable once NotAccessedField.Local
        private int _errorLine;
        private int _errorColumn;
        private string _errorMessage;

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
            _engine?.Dispose();
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
            var context = new ScriptContext();
            context.Var.Value1 = 1000;
            context.Var.Value2 = 234;
        
            const string script = "Var.Value1 + Var.Value2";
            var result = _engine.Eval<int>(script, context);
            Assert.Equal(1234, result);
        }

    }
}