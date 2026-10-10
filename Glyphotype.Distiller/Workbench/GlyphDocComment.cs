using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Glyphotype.Distiller.Workbench;

/// <summary>The <c>///</c> doc comment above a declaration in source, and the <see cref="GlyphDocumentation"/> it holds.</summary>
static class GlyphDocComment
{
    /// <summary>The doc comment just above <paramref name="declaration"/>, or null when it has none.</summary>
    public static SyntaxTrivia? Find(SyntaxNode declaration) =>
        declaration.GetLeadingTrivia().LastOrDefault(x => x.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia)) is var comment && comment != default
            ? comment
            : null;

    /// <summary>What the doc comment above <paramref name="declaration"/> says (see <see cref="GlyphDocumentation.Read"/>) - nothing at all when there's none.</summary>
    public static GlyphDocumentation.ReadComment Read(SyntaxNode declaration) =>
        GlyphDocumentation.Read(Find(declaration)?.ToFullString());

    /// <summary>
    /// The declarations in <paramref name="source"/> with a doc comment somewhere other than above them all - after their
    /// attributes, say - where <see cref="Find"/> doesn't look, so it's dropped.
    /// </summary>
    public static IReadOnlyList<string> Misplaced(string source) =>
        string.IsNullOrWhiteSpace(source)
            ? []
            : CSharpSyntaxTree.ParseText(source).GetRoot().DescendantNodes().OfType<BaseTypeDeclarationSyntax>()
                .Where(x => x.DescendantTrivia()
                    .Any(y => y.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia) && y.SpanStart > x.SpanStart && y.SpanStart < x.Identifier.SpanStart))
                .Select(x => x.Identifier.Text)
                .ToList();
}
