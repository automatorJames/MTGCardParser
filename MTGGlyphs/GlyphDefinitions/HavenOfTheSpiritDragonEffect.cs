namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Haven of the Spirit Dragon, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Haven of the Spirit Dragon</exampledoc>
[Dependent]
public class HavenOfTheSpiritDragonEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["return target dragon creature card or ugin planeswalker card from your graveyard to your hand"];
}
