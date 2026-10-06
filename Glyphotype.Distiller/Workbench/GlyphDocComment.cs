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

    /// <summary>What the doc comment above <paramref name="declaration"/> says - null when there's none.</summary>
    /// <exception cref="FormatException">The comment holds something a <see cref="GlyphDocumentation"/> can't.</exception>
    public static GlyphDocumentation Read(SyntaxNode declaration) =>
        Find(declaration) is { } comment ? GlyphDocumentation.FromComment(comment.ToFullString()) : null;
}
