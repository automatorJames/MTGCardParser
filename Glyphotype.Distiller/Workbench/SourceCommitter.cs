using Glyphotype.Attributes.Quantifiers;
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
    /// <summary>The attributes a definition expresses, by type name: any other on a rewritten declaration is lost.</summary>
    static readonly HashSet<string> _expressedAttributes =
    [
        nameof(DependentAttribute),
        nameof(AllowPartialClauseMatchAttribute),
        nameof(TokenizationOrderAttribute),
        nameof(RegexPatternAttribute),
        nameof(JoinedByAttribute),
        nameof(OptionalPluralAttribute),
        nameof(OptionalAttribute),
        nameof(AllowUnmatchedAttribute),
        nameof(TypeFilterAttribute),
        nameof(ColorAttribute),
        nameof(ReferentAttribute),
        nameof(SingularAttribute),
        nameof(PluralAttribute),
        nameof(RefersToAttribute),
    ];

    /// <summary>Whether the attribute written <paramref name="name"/> in source - with or without its "Attribute" suffix - is one a definition expresses.</summary>
    static bool IsExpressed(string name) =>
        _expressedAttributes.Contains(name.EndsWith("Attribute") ? name : name + "Attribute");

    /// <summary>How the comment above a declaration the Grammar Tools page wrote begins - followed by the date it was written.</summary>
    public const string CommitStampPrefix = "// Committed from Grammar Tools on ";

    /// <summary>The same, for a declaration exported from a workspace.</summary>
    public const string ExportStampPrefix = "// Exported from Grammar Tools on ";

    /// <summary>The comment written above a declaration committed on <paramref name="date"/>.</summary>
    public static string CommitStamp(DateTime date) => $"{CommitStampPrefix}{date:yyyy-MM-dd}";

    /// <summary>The comment written above a declaration exported on <paramref name="date"/>.</summary>
    public static string ExportStamp(DateTime date) => $"{ExportStampPrefix}{date:yyyy-MM-dd}";

    /// <summary>
    /// Plans the edits that turn the sources under <paramref name="sourceDirectory"/> (which declare
    /// <paramref name="committed"/>) into sources declaring <paramref name="working"/>. New files go in
    /// <paramref name="sourceDirectory"/> under <paramref name="namespace"/>. Each declaration written gets a
    /// <see cref="CommitStamp"/> for <paramref name="date"/> (today by default) above it, in place of any earlier stamp.
    /// </summary>
    public static SourceCommitPlan Plan(GrammarDefinition committed, GrammarDefinition working, string sourceDirectory, string @namespace, DateTime? date = null)
    {
        var stamp = CommitStamp(date ?? DateTime.Today);
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

            var text = stamp + Environment.NewLine + Write(change);

            if (existing is null)
            {
                var path = Path.Combine(sourceDirectory, change.Name + ".cs");

                if (!newFiles.TryGetValue(path, out var fileDeclarations))
                    newFiles[path] = fileDeclarations = [];

                fileDeclarations.Add(text);
                edits.Add(new(change, path, null, text, notes));
                continue;
            }

            var start = PreviousStamp(existing.Node)?.SpanStart ?? existing.Node.Span.Start;

            // A glyph's summary is part of its definition, so the doc comment above it is written along with it - unless
            // the comment holds more than a summary that's unchanged, which is then kept as it was.
            if (change.After is GlyphDefinition glyph)
            {
                var documentation = DocumentationSummary.Read(existing.Node);

                if (documentation.Comment is { } comment)
                {
                    start = Math.Min(start, comment.FullSpan.Start);

                    if (documentation.HoldsMore && documentation.Summary == glyph.Summary)
                        text = stamp + Environment.NewLine + Dedent(comment.ToFullString()) + GlyphSourceWriter.WriteGlyph(glyph with { Summary = null });
                    else if (documentation.HoldsMore)
                        notes.Add("The doc comment above it held more than a summary, and only the summary is written");
                }
            }

            var replacement = Reindent(text.TrimEnd(), existing.Indentation);

            if (existing.Node is ClassDeclarationSyntax classDeclaration)
                replacement = CarryOverUnexpressedMembers(classDeclaration, replacement, notes);

            notes.AddRange(existing.Node.AttributeLists
                .SelectMany(x => x.Attributes)
                .Select(x => x.Name.ToString())
                .Where(x => !IsExpressed(x))
                .Select(x => $"[{x}] isn't part of a definition, and is dropped"));

            AddSplice(spliceEdits, existing.Path, TextSpan.FromBounds(start, existing.Node.Span.End), replacement);
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

    /// <summary>
    /// The commit or export stamp just above <paramref name="node"/>, which a new one replaces - null when there's none.
    /// Only a doc comment can come between them: a glyph's stamp is written above its summary.
    /// </summary>
    static SyntaxTrivia? PreviousStamp(SyntaxNode node) =>
        node.GetLeadingTrivia().LastOrDefault(x => x.IsKind(SyntaxKind.SingleLineCommentTrivia)) is var comment
            && (comment.ToString().StartsWith(CommitStampPrefix, StringComparison.Ordinal) || comment.ToString().StartsWith(ExportStampPrefix, StringComparison.Ordinal))
            && node.GetLeadingTrivia().SkipWhile(x => x != comment).Skip(1).All(x => x.IsKind(SyntaxKind.EndOfLineTrivia) || x.IsKind(SyntaxKind.WhitespaceTrivia) || x.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
            ? comment
            : null;

    /// <summary><paramref name="text"/> with each line's indentation taken off, for <see cref="Reindent"/> to put back at the declaration's own.</summary>
    static string Dedent(string text) =>
        string.Join(Environment.NewLine, text.ReplaceLineEndings("\n").Split('\n').Select(x => x.TrimStart()));

    /// <summary>
    /// <paramref name="definition"/> with each glyph's <see cref="GlyphDefinition.Summary"/> read from the doc comment
    /// above its declaration under <paramref name="sourceDirectory"/> - which compiled types, being without their
    /// comments, can't give it.
    /// </summary>
    public static GrammarDefinition WithSummaries(GrammarDefinition definition, string sourceDirectory)
    {
        if (!Directory.Exists(sourceDirectory))
            return definition;

        var declarations = IndexDeclarations(sourceDirectory);

        return definition with
        {
            Glyphs = definition.Glyphs
                .Select(x => declarations.TryGetValue(x.Name, out var declaration) && declaration.Node is ClassDeclarationSyntax
                    ? x with { Summary = DocumentationSummary.Read(declaration.Node).Summary }
                    : x)
                .ToList(),
        };
    }

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
