using System;
using Xunit;

namespace IctBaden.Scripting.Test.CsharpScript
{
    public class ExpressionTests : IDisposable
    {
        private readonly ScriptEngine _engine;
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
        public void ContextValuesBeResolved()
        {
            var context = new ScriptContext();
            context.SetValue("Var1", 1000);
            context.SetValue("Var2", 234);

            const string script = "GetValue<int>(\"Var1\") + GetValue<int>(\"Var2\")";
            var result = _engine.Eval<int>(script, context);
            Assert.Equal(1234, result);
        }

    }
}