namespace Glyphotype.RegexGeneration.Presentation;

/// <summary>
/// One possible resolution of a <see cref="Glyphotype.GlyphPrimitives.DynamicGlyph"/> capture, offered as a
/// menu item when a <see cref="ClassSpan"/> with no single fixed <see cref="ClassSpan.NavigateTo"/> target is
/// clicked (see <see cref="ClassSpan.Resolutions"/>) - one per distinct concrete <see cref="Glyph"/> type that
/// capture actually resolved to somewhere in the corpus (<see cref="Glyphotype.GlyphAnalysisDTOs.TypeExpressions.DynamicCaptureTraceSummary.ResolvedTypeGlyphs"/>).
/// </summary>
/// <param name="Node">The resolved type's own root node - clicking this option navigates the C# Class view here.</param>
/// <param name="DataPath">
/// The resolved type's rebased data-path (e.g. <c>IfYouDo_Outcome_LifeChangeQuantity</c>) - the same
/// <c>Dynamic_ResolvedType</c> shape <see cref="Glyphotype.RegexGeneration.Presentation.DynamicSectionBuilder"/>
/// already rebases that same resolved instance's own bricks onto in the formatted regex column, so navigating
/// here and hovering its header lines up with that same expansion there.
/// </param>
/// <param name="Palette">The resolved type's own color, from the same palette every other span on this view shares.</param>
public record DynamicResolutionOption(NamedGroupNode Node, string DataPath, SpanStylePalette Palette);
