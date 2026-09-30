using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Glyphotype.Distiller.Workbench;

/// <summary>One declaration a commit writes, rewrites or deletes, with its source before and after (null where absent).</summary>
public sealed record DeclarationEdit(DefinitionChange Change, string FilePath, string Before, string After, IReadOnlyList<string> Notes);

/// <summary>One file a commit writes: <see cref="OriginalText"/> is null for a new file, <see cref="NewText"/> null for one deleted.</summary>
public sealed record FileWrite(string Path, string OriginalText, string NewText);

/// <summary>Everything a commit would do, for review before <see cref="SourceCommitter.Apply"/> does it.</summary>
public sealed record SourceCommitPlan(IReadOnlyList<DeclarationEdit> Declarations, IReadOnlyList<FileWrite> Files);

/// <summary>
/// Turns definition changes into edits of the C# sources that declare them. Each changed declaration is found by
/// name among the sources and replaced in place - only its own text, so the rest of its file, and its own leading
/// doc comment, stay exactly as they were. A new one gets a file of its own; a removed one is cut out, and a
/// file left declaring nothing is deleted.
/// <para>
/// A rewritten declaration is regenerated from its definition (see <see cref="GlyphSourceWriter"/>), so
/// formatting and comments inside it are not kept. Members a definition doesn't hold - a glyph's computed
/// properties, methods, constructors - are carried over verbatim. Attributes it doesn't hold are dropped, and
/// the plan says so.
/// </para>
/// </summary>
public static class SourceCommitter
{
    /// <summary>The attributes a definition expresses: any other on a rewritten declaration is lost.</summary>
    static readonly HashSet<string> _expressedAttributes =
        ["Dependent", "MustMatchWholeLine", "AllowPartialSegmentMatch", "TokenizationOrder", "RegexPattern", "JoinedBy", "OptionalPlural", "Optional", "TypeFilter", "Color"];

    /// <summary>
    /// Plans the edits that turn the sources under <paramref name="sourceDirectory"/> (which declare
    /// <paramref name="committed"/>) into sources declaring <paramref name="working"/>. New files go in
    /// <paramref name="sourceDirectory"/> under <paramref name="namespace"/>.
    /// </summary>
    public static SourceCommitPlan Plan(GrammarDefinition committed, GrammarDefinition working, string sourceDirectory, string @namespace)
    {
        var declarations = IndexDeclarations(sourceDirectory);
        var edits = new List<DeclarationEdit>();
        var spliceEdits = new Dictionary<string, List<(TextSpan Span, string Text)>>(StringComparer.OrdinalIgnoreCase);
        var newFiles = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

        foreach (var change in DefinitionDiff.Compare(committed, working))
        {
            List<string> notes = [];
            declarations.TryGetValue(change.Name, out var existing);

            if (change.Change == ChangeType.Removed)
            {
                if (existing is null)
                {
                    edits.Add(new(change, null, null, null, [$"No declaration of {change.Name} was found under {sourceDirectory}, so there's nothing to remove"]));
                    continue;
                }

                AddSplice(spliceEdits, existing.Path, existing.Node.FullSpan, "");
                edits.Add(new(change, existing.Path, existing.Node.ToString(), null, notes));
                continue;
            }

            var text = Write(change);

            if (existing is null)
            {
                var path = Path.Combine(sourceDirectory, change.Name + ".cs");

                if (!newFiles.TryGetValue(path, out var fileDeclarations))
                    newFiles[path] = fileDeclarations = [];

                fileDeclarations.Add(text);
                edits.Add(new(change, path, null, text, notes));
                continue;
            }

            var replacement = Reindent(text.TrimEnd(), existing.Indentation);

            if (existing.Node is ClassDeclarationSyntax classDeclaration)
                replacement = CarryOverUnexpressedMembers(classDeclaration, replacement, notes);

            notes.AddRange(existing.Node.AttributeLists
                .SelectMany(x => x.Attributes)
                .Select(x => x.Name.ToString())
                .Where(x => !_expressedAttributes.Contains(x) && !_expressedAttributes.Contains(x.EndsWith("Attribute") ? x[..^9] : x))
                .Select(x => $"[{x}] isn't part of a definition, and is dropped"));

            AddSplice(spliceEdits, existing.Path, existing.Node.Span, replacement);
            edits.Add(new(change, existing.Path, existing.Node.ToString(), replacement, notes));
        }

        List<FileWrite> files = [];

        foreach (var (path, splices) in spliceEdits)
        {
            var original = File.ReadAllText(path);
            var text = original;

            foreach (var (span, replacement) in splices.OrderByDescending(x => x.Span.Start))
                text = text[..span.Start] + replacement + text[span.End..];

            // A file whose last declaration was just removed goes too, rather than lingering as a bare namespace.
            var declaresNothing = !CSharpSyntaxTree.ParseText(text).GetRoot().DescendantNodes().OfType<BaseTypeDeclarationSyntax>().Any();
            files.Add(new(path, original, declaresNothing ? null : text));
        }

        foreach (var (path, fileDeclarations) in newFiles)
        {
            var original = File.Exists(path) ? File.ReadAllText(path) : null;
            var added = string.Join(Environment.NewLine, fileDeclarations);

            // Only if a file of that name already exists without declaring this - then it's appended to.
            var text = original is null
                ? $"namespace {@namespace};{Environment.NewLine}{Environment.NewLine}{added}"
                : original.TrimEnd() + Environment.NewLine + Environment.NewLine + added;

            files.Add(new(path, original, text));
        }

        return new(edits, files);
    }

    /// <summary>Writes (or deletes) every file in <paramref name="plan"/>.</summary>
    public static void Apply(SourceCommitPlan plan)
    {
        foreach (var file in plan.Files)
        {
            if (file.NewText is null)
            {
                File.Delete(file.Path);
                continue;
            }

            var newline = file.OriginalText is not null && !file.OriginalText.Contains("\r\n") ? "\n" : "\r\n";
            File.WriteAllText(file.Path, file.NewText.ReplaceLineEndings(newline), new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
        }
    }

    static string Write(DefinitionChange change) =>
        change.After switch
        {
            GlyphDefinition glyph => GlyphSourceWriter.WriteGlyph(glyph),
            VocabularyDefinition vocabulary => GlyphSourceWriter.WriteVocabulary(vocabulary),
            string marker => GlyphSourceWriter.WriteMarker(marker),
            _ => throw new NotSupportedException($"Can't write {change.Kind} {change.Name}"),
        };

    /// <summary>
    /// <paramref name="replacement"/> with <paramref name="original"/>'s members that no definition holds - anything
    /// but the nib-bound auto-properties and the <c>Nibs</c>/<c>Joiner</c> overrides - carried over verbatim.
    /// </summary>
    static string CarryOverUnexpressedMembers(ClassDeclarationSyntax original, string replacement, List<string> notes)
    {
        var unexpressed = original.Members.Where(x => !IsExpressedMember(x)).ToList();

        if (unexpressed.Count == 0)
            return replacement;

        notes.Add($"Kept members a definition doesn't hold: {string.Join(", ", unexpressed.Select(Describe))}");

        var members = string.Concat(unexpressed.Select(x => x.ToFullString())).TrimEnd();
        var closing = Environment.NewLine + original.GetLeadingTrivia().ToString().Split('\n').Last() + "}";

        // The writer gives a member-less class a ";" body; one gaining carried-over members needs braces again.
        if (replacement.EndsWith(';'))
            return replacement[..^1] + closing[..^1] + "{" + Environment.NewLine + members + closing;

        var lastBrace = replacement.LastIndexOf('}');
        return replacement[..lastBrace].TrimEnd() + Environment.NewLine + Environment.NewLine + members + closing;
    }

    static bool IsExpressedMember(MemberDeclarationSyntax member) =>
        member is PropertyDeclarationSyntax property
        && (property.Modifiers.Any(SyntaxKind.OverrideKeyword)
            ? property.Identifier.Text is nameof(Glyph.Nibs) or nameof(Glyph.Joiner)
            : property.Modifiers.Any(SyntaxKind.PublicKeyword)
                && property.AccessorList?.Accessors.Any(x => x.IsKind(SyntaxKind.SetAccessorDeclaration) && !x.Modifiers.Any()) == true);

    static string Describe(MemberDeclarationSyntax member) =>
        member switch
        {
            PropertyDeclarationSyntax property => property.Identifier.Text,
            MethodDeclarationSyntax method => method.Identifier.Text + "()",
            ConstructorDeclarationSyntax => "constructor",
            FieldDeclarationSyntax field => string.Join(", ", field.Declaration.Variables.Select(x => x.Identifier.Text)),
            _ => member.Kind().ToString(),
        };

    static string Reindent(string text, string indentation) =>
        indentation.Length == 0
            ? text
            : string.Join(Environment.NewLine, text.ReplaceLineEndings("\n").Split('\n').Select((line, i) => i == 0 || line.Length == 0 ? line : indentation + line));

    static void AddSplice(Dictionary<string, List<(TextSpan, string)>> spliceEdits, string path, TextSpan span, string text)
    {
        if (!spliceEdits.TryGetValue(path, out var splices))
            spliceEdits[path] = splices = [];

        splices.Add((span, text));
    }

    sealed record Declaration(string Path, BaseTypeDeclarationSyntax Node, string Indentation);

    /// <summary>Every top-level type declaration in the C# files under <paramref name="sourceDirectory"/> (bin/obj aside), by name.</summary>
    static Dictionary<string, Declaration> IndexDeclarations(string sourceDirectory)
    {
        var index = new Dictionary<string, Declaration>();

        foreach (var path in Directory.EnumerateFiles(sourceDirectory, "*.cs", SearchOption.AllDirectories))
        {
            var relative = Path.GetRelativePath(sourceDirectory, path).Replace('\\', '/');

            if (relative.StartsWith("bin/") || relative.StartsWith("obj/") || relative.Contains("/bin/") || relative.Contains("/obj/"))
                continue;

            var text = SourceText.From(File.ReadAllText(path));
            var root = CSharpSyntaxTree.ParseText(text).GetRoot();

            foreach (var node in root.DescendantNodes().OfType<BaseTypeDeclarationSyntax>().Where(x => x.Parent is not BaseTypeDeclarationSyntax))
            {
                var line = text.Lines.GetLineFromPosition(node.SpanStart);
                var indentation = text.ToString(TextSpan.FromBounds(line.Start, node.SpanStart));

                // Declarations are matched by name, so two of one name would make a commit ambiguous.
                if (!index.TryAdd(node.Identifier.Text, new(path, node, string.IsNullOrWhiteSpace(indentation) ? indentation : "")))
                    throw new InvalidOperationException($"{node.Identifier.Text} is declared more than once under {sourceDirectory} ({index[node.Identifier.Text].Path}, {path})");
            }
        }

        return index;
    }
}
