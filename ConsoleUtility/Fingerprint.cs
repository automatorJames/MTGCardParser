using System.Security.Cryptography;
using System.Text;
using Glyphotype.StaticRegistry;

namespace ConsoleUtility;

/// <summary>
/// Writes a fingerprint of every type's regex and brick sequence, one line per type, so a change to how graphs are
/// built can be checked for building exactly what it built before: <c>regexes [path]</c>.
/// </summary>
static class Fingerprint
{
    public static void Run(string path)
    {
        var grammar = GlyphGrammar.Default;
        var lines = new List<string>();

        foreach (var type in grammar.Types.OrderBy(x => x.FullName, StringComparer.Ordinal))
        {
            var graph = GlyphTypeCache.GetRegexGraph(type);
            var regex = graph.BuiltRegex.MinifiedRegex;
            var bricks = new StringBuilder();

            foreach (var brick in graph.BuiltRegex.Bricks)
                bricks.Append(brick.GetType().Name).Append('|').Append(brick.Regex).Append('|').Append(brick.FullyQualifiedName).Append('|').Append(brick.NestedDepth).Append('\n');

            lines.Add($"{type.Name}\t{regex.Length}\t{Hash(regex)}\t{graph.NamedGroupFlatGraph.Count}\t{Hash(bricks.ToString())}\t{graph.StartChars}\t{graph.SpansClauses}");
        }

        File.WriteAllLines(path, lines);
        Console.WriteLine($"Wrote {lines.Count} fingerprints to {path}");
    }

    static string Hash(string text) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(text)))[..16];
}
