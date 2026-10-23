namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Mu Yanling, Sky Dancer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mu Yanling, Sky Dancer</exampledoc>
[Dependent]
public class MuYanlingSkyDancerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you get an emblem with \"islands you control have '{t}: draw a card"];
}
