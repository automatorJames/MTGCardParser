namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Sage's Nouliths, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sage's Nouliths</exampledoc>
[TokenizationOrder(-1)]
public class SagesNoulithsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["equipped creature gets +1/+0, has \"whenever this creature attacks, untap target attacking creature,\" and is a cleric in addition to its other types"];
}
