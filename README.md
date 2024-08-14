# IctBaden.Scripting
Script engine abstraction easy to use in applications.

Simple usage:

```csharp
	var engine = ScriptFactory.CreateCsharpScriptEngine();
	var result = engine.Eval<int>("1 + 1");
```

Optional user error handling:
```csharp
	var engine = ScriptFactory.CreateCsharpScriptEngine();
	engine.ScriptError += (line, column, message) => Console.WriteLine($"({line},{column}): {message})");
```

Optional context and variable handling:
```csharp
	var engine = ScriptFactory.CreateCsharpScriptEngine();
    var context = new ScriptContext();
    context.SetVar("Value1", 111);
    context.SetVar("Value2", 222);
    var result = engine.Eval<int>("Var.Value1 + Var.Value2");
```
