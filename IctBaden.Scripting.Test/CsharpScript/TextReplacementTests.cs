using System;
using Xunit;

namespace IctBaden.Scripting.Test.CsharpScript
{
    public class TextReplacementTests
    {
        private readonly ScriptEngine _engine;

        public TextReplacementTests()
        {
            _engine = ScriptFactory.CreateCsharpScriptEngine();
            _engine.ScriptError += (line, column, message) => Console.WriteLine($"({line},{column}): {message})");
        }

        [Fact]
        public void ContextValuesBeResolved()
        {
            var context = new ScriptContext();
            context.SetValue("TargetName", "Frank");

            const string text = "Hallo, ich bin {{GetValue<string>(\"TargetName\")}} !";
            var result = _engine.ReplaceExpressions(text, context);
            Assert.Equal("Hallo, ich bin Frank !", result);
        }

    }
}