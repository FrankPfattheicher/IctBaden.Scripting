using System;
using IctBaden.Scripting.Engines;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UseNameofExpression

namespace IctBaden.Scripting
{
    public static class ScriptFactory
    {
        public static ScriptEngine CreateCsharpScriptEngine() => CreateScriptEngine("csx", new string[0]);
        public static ScriptEngine CreateCsharpScriptEngine(string[] imports) => CreateScriptEngine("csx", imports);

        public static ScriptEngine CreateScriptEngine(string language, string[] imports)
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
