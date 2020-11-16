using System;
using IctBaden.Scripting.Engines;

namespace IctBaden.Scripting
{
    public static class ScriptFactory
    {
        public static ScriptEngine CreateCsharpScriptEngine() => CreateScript("csx", new string[0]);
        public static ScriptEngine CreateCsharpScriptEngine(string[] imports) => CreateScript("csx", imports);

        public static ScriptEngine CreateScript(string language, string[] imports)
        {
            switch (language.ToLower())
            {
                case "csx":
                    return new RoslynCsharpScript(imports);
            }
            throw new ArgumentException("Script language not supported", "language");
        }
    }
}
