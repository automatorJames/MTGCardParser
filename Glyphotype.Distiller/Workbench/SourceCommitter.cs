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

/// <summary>A definition a commit adds under a name some type in the project already has - one no definition holds, so the commit can't simply rewrite it.</summary>
public sealed record NameConflict(DefinitionChange Change, string ExistingPath);

/// <summary>What a commit does about a <see cref="NameConflict"/>.</summary>
public enum NameConflictResolution
{
    /// <summary>Nothing: the new declaration is written alongside the existing type, and the two share a name.</summary>
    AllowConflicts,

    /// <summary>The existing type is renamed, with <see cref="SourceCommitter.LegacySuffix"/>, where it's declared - code referring to the name then refers to the new declaration.</summary>
    RenameExisting,

    /// <summary>The new definition is renamed, with <see cref="SourceCommitter.NewSuffix"/>, along with every reference to it - in the working grammar too.</summary>
    RenameIncoming,
}

/// <summary>Everything a commit would do, for review before <see cref="SourceCommitter.Apply"/> does it.</summary>
/// <param name="Definition">The grammar the sources will declare: the working one, unless <see cref="NameConflictResolution.RenameIncoming"/> renamed some of it.</param>
/// <param name="Conflicts">The names the commit adds that some type in the project already has - for the person to resolve before committing, unless <paramref name="Resolution"/> already does.</param>
public sealed record SourceCommitPlan(
    GrammarDefinition Definition,
    IReadOnlyList<DeclarationEdit> Declarations,
    IReadOnlyList<FileWrite> Files,
    IReadOnlyList<NameConflict> Conflicts,
    NameConflictResolution? Resolution)
{
    /// <summary>Whether the plan can be applied as it is: it has no conflicts, or says what to do about them.</summary>
    public bool IsResolved => Conflicts.Count == 0 || Resolution is not null;
}

/// <summary>
/// Turns definition changes into edits of the C# sources that declare them. Each changed declaration is found by
/// name anywhere in the project the sources belong to - so a vocabulary declared outside the source directory is
/// still rewritten where it is - and replaced in place: only its own text, so the rest of its file, and its own
/// leading doc comment, stay exactly as they were. A new one gets a file of its own - a vocabulary's under
/// <see cref="VocabularyDirectory"/>, apart from the glyphs; a removed one is cut out, and a file left declaring
/// nothing is deleted.
/// <para>
/// A rewritten declaration is regenerated from its definition (see <see cref="GlyphSourceWriter"/>), so
/// formatting and comments inside it are not kept. Members a definition doesn't hold - a glyph's computed
/// properties, methods, constructors - are carried over verbatim. Attributes it doesn't hold are dropped, and
/// the plan says so.
/// </para>
/// </summary>
public static class SourceCommitter
{
    /// <summary>The subdirectory of the source directory a new vocabulary's file goes in.</summary>
    public const string VocabularyDirectory = "Enums";

    /// <summary>What <see cref="NameConflictResolution.RenameExisting"/> and <see cref="NameConflictResolution.RenameIncoming"/> append to a name.</summary>
    public const string LegacySuffix = "_legacy";
    public const string NewSuffix = "_new";

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

    /// <summary>
    /// Plans the edits that turn the sources of the project <paramref name="sourceDirectory"/> belongs to (which
    /// declare <paramref name="committed"/>) into sources declaring <paramref name="working"/>. New files go in
    /// <paramref name="sourceDirectory"/> - a vocabulary's in its <see cref="VocabularyDirectory"/> - under
    /// <paramref name="namespace"/>.
    /// </summary>
    /// <param name="resolution">What to do about any <see cref="SourceCommitPlan.Conflicts"/> - null to leave it to the person, in which case the plan previews them as allowed.</param>
    public static SourceCommitPlan Plan(GrammarDefinition committed, GrammarDefinition working, string sourceDirectory, string @namespace, NameConflictResolution? resolution = null)
    {
        var declarations = IndexDeclarations(FindProjectDirectory(sourceDirectory));
        var conflicts = FindConflicts(committed, working, declarations);
        var renamedFrom = new Dictionary<string, string>();

        if (resolution == NameConflictResolution.RenameIncoming)
        {
            foreach (var conflict in conflicts)
            {
                var renamed = UnusedName(conflict.Change.Name + NewSuffix, working, declarations);
                working = working.WithRenamed(conflict.Change.Name, renamed);
                renamedFrom[renamed] = conflict.Change.Name;
            }
        }

        var edits = new List<DeclarationEdit>();
        var spliceEdits = new Dictionary<string, List<(TextSpan Span, string Text)>>(StringComparer.OrdinalIgnoreCase);
        var newFiles = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

        foreach (var change in DefinitionDiff.Compare(committed, working))
        {
            List<string> notes = [];
            var existing = Find(declarations, change.Name);

            // An added definition's name belongs to no definition, so whatever has it is some other type: left be, or renamed.
            if (change.Change == ChangeType.Added && existing is not null)
            {
                if (resolution == NameConflictResolution.RenameExisting)
                {
                    var legacy = UnusedName(change.Name + LegacySuffix, working, declarations);
                    AddSplice(spliceEdits, existing.Path, existing.Node.Identifier.Span, legacy);
                    notes.Add($"The existing {change.Name} in {existing.Path} is renamed {legacy}: code that referred to {change.Name} now refers to this one");
                }
                else
                {
                    notes.Add($"{existing.Path} already declares a type named {change.Name}");
                }

                existing = null;
            }

            if (renamedFrom.TryGetValue(change.Name, out var original))
                notes.Add($"Renamed from {original}, a name a type in the project already has - in the working grammar too");

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
                var path = change.Kind == DefinitionKind.Vocabulary
                    ? Path.Combine(sourceDirectory, VocabularyDirectory, change.Name + ".cs")
                    : Path.Combine(sourceDirectory, change.Name + ".cs");

                if (!newFiles.TryGetValue(path, out var fileDeclarations))
                    newFiles[path] = fileDeclarations = [];

                fileDeclarations.Add(text);
                edits.Add(new(change, path, null, text, notes));
                continue;
            }

            var start = existing.Node.Span.Start;

            // A glyph's doc comment holds its definition's documentation, so it's written again along with the rest - keeping
            // whatever else the comment held after it.
            if (change.After is GlyphDefinition glyph && GlyphDocComment.Find(existing.Node) is { } comment)
            {
                start = comment.FullSpan.Start;
                text = GlyphSourceWriter.WriteGlyph(glyph, GlyphDocComment.Read(existing.Node).Rest);
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

        return new(working, edits, files, conflicts, conflicts.Count > 0 ? resolution : null);
    }

    /// <summary>The definitions <paramref name="working"/> adds under a name some type in the project already has.</summary>
    static List<NameConflict> FindConflicts(GrammarDefinition committed, GrammarDefinition working, Dictionary<string, List<Declaration>> declarations) =>
        DefinitionDiff.Compare(committed, working)
            .Where(x => x.Change == ChangeType.Added && declarations.ContainsKey(x.Name))
            .Select(x => new NameConflict(x, declarations[x.Name][0].Path))
            .ToList();

    /// <summary><paramref name="name"/> - or, should a definition or a type in the project have it, the first of name2, name3, … that none does.</summary>
    static string UnusedName(string name, GrammarDefinition definition, Dictionary<string, List<Declaration>> declarations)
    {
        bool IsUsed(string candidate) =>
            declarations.ContainsKey(candidate)
            || definition.Glyphs.Any(x => x.Name == candidate)
            || definition.Vocabularies.Any(x => x.Name == candidate)
            || definition.Markers.Contains(candidate);

        var unused = name;

        for (int i = 2; IsUsed(unused); i++)
            unused = name + i;

        return unused;
    }

    /// <summary>
    /// The directory of the project <paramref name="sourceDirectory"/> belongs to - the nearest one holding a
    /// <c>.csproj</c>, at or above it - or <paramref name="sourceDirectory"/> itself when it belongs to none.
    /// </summary>
    static string FindProjectDirectory(string sourceDirectory)
    {
        for (var directory = new DirectoryInfo(sourceDirectory); directory is not null; directory = directory.Parent)
        {
            if (directory.Exists && directory.EnumerateFiles("*.csproj").Any())
                return directory.FullName;
        }

        return sourceDirectory;
    }

    /// <summary>Writes (or deletes) every file in <paramref name="plan"/> - which must be resolved (see <see cref="SourceCommitPlan.IsResolved"/>).</summary>
    public static void Apply(SourceCommitPlan plan)
    {
        if (!plan.IsResolved)
            throw new InvalidOperationException($"Types in the project already have names this commit adds ({string.Join(", ", plan.Conflicts.Select(x => x.Change.Name))}) - say what to do about them first");

        foreach (var file in plan.Files)
        {
            if (file.NewText is null)
            {
                File.Delete(file.Path);
                continue;
            }

            Directory.CreateDirectory(Path.GetDirectoryName(file.Path));

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
    /// <paramref name="definition"/> with each glyph's <see cref="GlyphDefinition.Documentation"/> read from the doc comment
    /// above its declaration under <paramref name="sourceDirectory"/> - which compiled types, being without their
    /// comments, can't give it.
    /// </summary>
    public static GrammarDefinition WithDocumentation(GrammarDefinition definition, string sourceDirectory)
    {
        if (!Directory.Exists(sourceDirectory))
            return definition;

        var declarations = IndexDeclarations(sourceDirectory);

        return definition with
        {
            Glyphs = definition.Glyphs
                .Select(x => declarations.TryGetValue(x.Name, out var found) && found is [{ Node: ClassDeclarationSyntax } declaration]
                    ? x with { Documentation = GlyphDocComment.Read(declaration.Node).Documentation }
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

    /// <summary>The one declaration named <paramref name="name"/> - null for none, and an error for more than one, which would make a commit ambiguous.</summary>
    static Declaration Find(Dictionary<string, List<Declaration>> declarations, string name) =>
        !declarations.TryGetValue(name, out var found) ? null
        : found.Count == 1 ? found[0]
        : throw new InvalidOperationException($"{name} is declared more than once ({string.Join(", ", found.Select(x => x.Path))}), so a commit can't tell which to change - rename or remove all but one");

    /// <summary>Every top-level type declaration in the C# files under <paramref name="sourceDirectory"/> (bin/obj aside), by name.</summary>
    static Dictionary<string, List<Declaration>> IndexDeclarations(string sourceDirectory)
    {
        var index = new Dictionary<string, List<Declaration>>();

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

                // Declarations are matched by name, so two of one name make a commit ambiguous - but only one that changes it (see Find).
                if (!index.TryGetValue(node.Identifier.Text, out var named))
                    index[node.Identifier.Text] = named = [];

                named.Add(new(path, node, string.IsNullOrWhiteSpace(indentation) ? indentation : ""));
            }
        }

        return index;
    }
}
