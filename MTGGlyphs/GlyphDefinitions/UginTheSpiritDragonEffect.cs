namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Ugin, the Spirit Dragon, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ugin, the Spirit Dragon</exampledoc>
[Dependent]
public class UginTheSpiritDragonEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile each permanent with mana value x or less that's one or more colors"];
}
