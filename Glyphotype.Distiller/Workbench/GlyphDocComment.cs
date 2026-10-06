using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

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
}
