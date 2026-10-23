namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Teferi, Temporal Pilgrim, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Teferi, Temporal Pilgrim</exampledoc>
[TokenizationOrder(-1)]
public class TeferiTemporalPilgrimRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then they shuffle each nonland permanent they control into its owner's library"];
}
