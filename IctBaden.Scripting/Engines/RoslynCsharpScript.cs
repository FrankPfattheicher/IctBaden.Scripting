using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using IctBaden.Framework.AppUtils;
using IctBaden.Framework.Types;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Binder = Microsoft.CSharp.RuntimeBinder.Binder;

// ReSharper disable CommentTypo

namespace IctBaden.Scripting.Engines;

/// <summary>
/// https://carljohansen.wordpress.com/2020/05/09/compiling-expression-trees-with-roslyn-without-memory-leaks-2/
/// </summary>
public class RoslynCsharpScript : ScriptEngine
{
    // ReSharper disable once UnusedMember.Local
    private AssemblyInfo _assemblyInfo = AssemblyInfo.Default;
    private readonly ScriptOptions _options;
    private readonly Dictionary<string, Script<object>> _scripts = new Dictionary<string, Script<object>>();

#pragma warning disable SYSLIB1045
    private static readonly Regex DecodeErrorMessage = new(@"^\(([0-9]+),([0-9]+)\): (.*)$", RegexOptions.Compiled);
#pragma warning restore SYSLIB1045
    
    private static readonly string[] DefaultImports =
    [
        "System",
        "System.Diagnostics",
        "System.IO",
        "System.Linq",
        "System.Net",
        "System.Text",
        "Microsoft.CSharp",
        "IctBaden.Framework",
        "IctBaden.Framework.AppUtils"
    ];

    public RoslynCsharpScript(string[] userImports)
    {
        var imports = DefaultImports
            .Concat(userImports)
            .ToArray();

        var userRefs = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => userImports.Any(ui => ui == a.GetName().Name)) 
            .ToArray();
        
        var currentDomainRefs = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => a.GetName().Name!.StartsWith("System.")
                        && a is { IsDynamic: false, ReflectionOnly: false }
                        && a.DefinedTypes.Any())
            .Union(
            [
                Assembly.GetEntryAssembly(),
                Assembly.GetCallingAssembly(),
                typeof(Binder).Assembly
            ])
            .Union(userRefs)
            .ToArray();

        _options = ScriptOptions.Default
            .WithImports(imports)
            .AddReferences(currentDomainRefs);
    }

    public override T Eval<T>(string expression, object? context = null)
    {
        LastError = string.Empty;

        try
        {
            Script<object> script;
            if (_scripts.TryGetValue(expression, out var existing))
            {
                script = existing;
            }
            else
            {
                var evalExpression = expression;
                
                if (context is ScriptContext sc)
                {
                    var inits = string.Empty;
                    foreach (var valueName in sc.ValueNames)
                    {
                        var value = sc[valueName];
                        var valueText = value is string
                            ? "\"" + value + "\""
                            : value?.ToString() ?? string.Empty;
                        inits = $"var {valueName} = {valueText};"
                                + Environment.NewLine + inits;
                    }

                    evalExpression = inits + $"return {expression};";
                }

                script = context != null
                    ? CSharpScript.Create(evalExpression, _options, context.GetType())
                    : CSharpScript.Create(evalExpression, _options);

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

            if (result?.ReturnValue == null)
            {
                return (T)UniversalConverter.ConvertToType(0, typeof(T))!;
            }

            return (T)UniversalConverter.ConvertToType(result.ReturnValue, typeof(T))!;
        }
        catch (Exception ex)
        {
            var line = 0;
            var column = 0;
            var message = ex.Message;

            // sample: (1,4): error CS1733: Ausdruck erwartet.
            var decodeMessage = DecodeErrorMessage 
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

        return (T)UniversalConverter.ConvertToType(string.Empty, typeof(T))!;
    }

    private bool OnScriptException(Exception arg)
    {
        LastError = arg.Message;
        return OnScriptError(0, 0, arg.Message);
    }
}