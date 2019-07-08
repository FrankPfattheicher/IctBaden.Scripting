using System;
using IctBaden.Scripting.Engines;

namespace IctBaden.Scripting
{
    public static class ScriptFactory
    {
        public static ScriptEngine CreateCsharpScriptEngine() => CreateScript("csx");

        public static ScriptEngine CreateScript(string language)
        {
            switch (language.ToLower())
            {
                case "csx":
                    return new RoslynCsharpScript();
            }
            throw new ArgumentException("Script language not supported", "language");
        }
    }
}
