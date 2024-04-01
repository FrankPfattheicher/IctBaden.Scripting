using Xunit;

namespace IctBaden.Scripting.Test;

public class CreateEngineTests
{
    [Fact]
    public void CreatingCsharpScriptEngineShouldSucceed()
    {
        using var engine = ScriptFactory.CreateCsharpScriptEngine();

        Assert.NotNull(engine);
    }

}