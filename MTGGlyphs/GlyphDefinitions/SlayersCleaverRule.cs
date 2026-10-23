namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Slayer's Cleaver, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Slayer's Cleaver</exampledoc>
[TokenizationOrder(-1)]
public class SlayersCleaverRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["equipped creature gets +3/+1 and must be blocked by an eldrazi if able"];
}
