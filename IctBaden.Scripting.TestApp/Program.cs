using System;
using IctBaden.Scripting.Engines;

namespace IctBaden.Scripting.TestApp;

internal static class Program
{
    private static long _count = 100000;
        
    private static void Main()
    {
        Console.WriteLine("IctBaden.Scripting.Test");

        using var engine = new RoslynCsharpScript(Array.Empty<string>());
            
        // ReSharper disable once UseObjectOrCollectionInitializer
        var context = new ScriptContext();
        context.Var.TestString = "Als Google Doodle wird die grafische Veränderung des Firmenlogos des US-amerikanischen Unternehmens Google LLC bezeichnet";

        Console.WriteLine();
        Console.WriteLine("1 - 100.000");
        Console.WriteLine("5 - 500.000");
        Console.WriteLine("! - 10.000.000");
        Console.WriteLine("e - eval");
        Console.WriteLine("c - compile");
        Console.WriteLine("r - run");
            
        while (true)
        {
            var cmd = Console.ReadLine();
            if(string.IsNullOrEmpty(cmd)) break;

            switch (cmd)
            {
                case "1":
                    _count = 100000;
                    break;
                case "5":
                    _count = 500000;
                    break;
                case "!":
                    _count = 10000000;
                    break;
                case "e":
                    Run(_ => engine.Eval<string>("1 + 1"));
                    break;
                case "c":
                    Run(_ => engine.Eval<string>("1 + ix"));
                    break;
            }
        }
        Console.WriteLine("EXIT.");
    }

    private static void Run(Action<long> action)
    {
        Console.WriteLine();
        for(var ix = 0; ix < _count; ix++)
        {
            if (ix % 100 == 0)
            {
                Console.Write($"{ix}\r");
            }
            action(ix);
        }
        Console.WriteLine();
        Console.WriteLine("done.");
    }
        
        
}