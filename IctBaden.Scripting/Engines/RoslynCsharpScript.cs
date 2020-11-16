using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using IctBaden.Framework.AppUtils;
using IctBaden.Framework.Types;

// ReSharper disable CommentTypo

namespace IctBaden.Scripting.Engines
{
    internal class RoslynCsharpScript : ScriptEngine
    {
        // ReSharper disable once UnusedMember.Local
        private AssemblyInfo _assemblyInfo = AssemblyInfo.Default;
        private ScriptOptions _options;
        private Dictionary<string, Script<object>> _scripts = new Dictionary<string, Script<object>>(); 

        public RoslynCsharpScript(string[] userImports)
        {
            var imports = new[]
                {
                    "System",
                    "System.Diagnostics",
                    "System.IO",
                    "System.Linq",
                    "System.Net",
                    "System.Text",
                    "IctBaden.Framework.AppUtils"
                }.Concat(userImports)
                .ToArray();
            
            var refs = new[]
            {
                Assembly.GetEntryAssembly(),
                Assembly.GetCallingAssembly()
            };

            _options = ScriptOptions.Default
                .WithImports(imports)
                .AddReferences(refs);
        }

        public override T Eval<T>(string expression, object context = null)
        {
            LastError = string.Empty;

            try
            {
                Script<object> script;
                if (_scripts.ContainsKey(expression))
                {
                    script = _scripts[expression];
                }
                else
                {
                    script = (context != null)
                        ? CSharpScript.Create(expression, _options, context.GetType())
                        : CSharpScript.Create(expression, _options);

                    var compile = script.Compile();
                    if (compile.Any())
                    {
                        LastError = string.Join(Environment.NewLine, compile);
                    }
                    else
                    {
                        _scripts.Add(expression, script);
                    }
                }

                var result = script.RunAsync(context, OnScriptException).Result;

                return (T) UniversalConverter.ConvertToType(result.ReturnValue, typeof(T));
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

            return (T) UniversalConverter.ConvertToType(string.Empty, typeof(T));
        }

        private bool OnScriptException(Exception arg)
        {
            LastError = arg.Message;
            return OnScriptError(0, 0, arg.Message);
        }
    }
}