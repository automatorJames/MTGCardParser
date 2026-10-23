namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Gideon's Intervention, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Gideon's Intervention</exampledoc>
[TokenizationOrder(-1)]
public class GideonsInterventionRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["prevent all damage that would be dealt to you and permanents you control by sources with the chosen name"];
}
