namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Rufus Shinra, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rufus Shinra</exampledoc>
[Dependent]
public class RufusShinraEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you don't control a creature named darkstar, create darkstar, a legendary 2/2 white and black dog creature token"];
}
