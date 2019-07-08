using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using IctBaden.Framework.AppUtils;
using IctBaden.Framework.Types;

namespace IctBaden.Scripting.Engines
{
    internal class RoslynCsharpScript : ScriptEngine
    {
        // ReSharper disable once UnusedMember.Local
        private Platform _platform = SystemInfo.Platform;

        public override T Eval<T>(string expression, ScriptContext context = null)
        {
            LastError = string.Empty;

            var refs = new[]
            {
                Assembly.GetEntryAssembly(),
                Assembly.GetCallingAssembly()
            };
            var imports = new[]
            {
                "System",
                "System.Diagnostics",
                "System.IO",
                "System.Linq",
                "System.Net",
                "System.Text",
                "IctBaden.Framework.AppUtils"
            };
            var options = ScriptOptions.Default
                .WithImports(imports)
                .AddReferences(refs);

            var script = (context != null) 
                        ? CSharpScript.Create(expression, options, typeof(ScriptContext))
                        : CSharpScript.Create(expression, options);

            try
            {
                var compile = script.Compile();
                if (compile.Any())
                {
                    LastError = string.Join(Environment.NewLine, compile);
                }

                var result = script.RunAsync(context, OnScriptException).Result;

                return (T)UniversalConverter.ConvertToType(result.ReturnValue, typeof(T));
            }
            catch (Exception ex)
            {
                var line = 0;
                var column = 0;
                var message = ex.Message;

                // (1,4): error CS1733: Ausdruck erwartet.
                var decodeMessage = new Regex(@"^\(([0-9]+),([0-9]+)\): (.*)$")
                    .Match(ex.Message);
                if (decodeMessage.Success)
                {
                    line = int.Parse(decodeMessage.Groups[1].Value);
                    column = int.Parse(decodeMessage.Groups[2].Value);
                    message = decodeMessage.Groups[3].Value;
                }

                LastError = message;
                OnScriptError(line, column, message);
            }

            return (T)UniversalConverter.ConvertToType(string.Empty, typeof(T));
        }

        private bool OnScriptException(Exception arg)
        {
            LastError = arg.Message;
            return OnScriptError(0, 0, arg.Message);
        }
    }
}