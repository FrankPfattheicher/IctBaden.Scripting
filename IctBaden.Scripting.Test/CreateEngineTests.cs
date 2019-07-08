using System;
using Xunit;

namespace IctBaden.Scripting.Test
{
    public class CreateEngineTests
    {
        [Fact]
        public void CreatingCsharpScriptEngineShouldSucceed()
        {
            var engine = ScriptFactory.CreateCsharpScriptEngine();

            Assert.NotNull(engine);
        }

    }
}
