using MTGEngine.Core;
using MTGRulesIR.Model;

namespace ConsoleUtility;

/// <summary>
/// <c>interpret rules.jsonl [out-dir]</c>: reads a lowered corpus and reports what of it the engine's interpreter can
/// execute - lines and cards whose every node the engine handles - and the node kinds that most often stand in the
/// way (see <see cref="InterpretReport"/>). Written as <c>interpret-report.txt</c> next to the input unless a directory is given.
/// </summary>
static class Interpret
{
    public static void Run(string rulesPath, string outDir)
    {
        var report = new InterpretReport();
        foreach (var line in File.ReadLines(rulesPath))
            if (!string.IsNullOrWhiteSpace(line))
                report.Add(IrJson.Deserialize<CardRules>(line));

        var text = report.ToText();
        var target = Path.Combine(outDir ?? Path.GetDirectoryName(Path.GetFullPath(rulesPath)) ?? ".", "interpret-report.txt");
        File.WriteAllText(target, text);
        Console.WriteLine(text);
    }
}
