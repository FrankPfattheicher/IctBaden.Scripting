using System;
using System.Diagnostics;
using Xunit;
using Xunit.Abstractions;

namespace IctBaden.Scripting.Test.CsharpScript
{
    public class PerformanceTests
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly ScriptEngine _engine;

        public PerformanceTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _engine = ScriptFactory.CreateCsharpScriptEngine();
            _engine.ScriptError += (line, column, message) => Console.WriteLine($"({line},{column}): {message})");
        }

        [Fact]
        public void CompilationShouldBeCached()
        {
            const string script = "1 + 2 + 3 + 4";
            
            var watch1 = new Stopwatch();
            watch1.Start();
            var result = _engine.Eval<string>(script);
            watch1.Stop();
            Assert.Equal("10", result);
            
            _testOutputHelper.WriteLine($"Execution 1 took {watch1.ElapsedMilliseconds}ms");
            
            var watch2 = new Stopwatch();
            watch2.Start();
            result = _engine.Eval<string>(script);
            watch2.Stop();
            Assert.Equal("10", result);
            _testOutputHelper.WriteLine($"Execution 2 took {watch2.ElapsedMilliseconds}ms");
            
            Assert.True(watch2.ElapsedMilliseconds < watch1.ElapsedMilliseconds);
        }

    }
}
