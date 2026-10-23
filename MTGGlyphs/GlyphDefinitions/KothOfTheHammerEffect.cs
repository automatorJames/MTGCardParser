namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Koth of the Hammer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Koth of the Hammer</exampledoc>
[Dependent]
public class KothOfTheHammerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you get an emblem with \"mountains you control have '{t}: this land deals 1 damage to any target"];
}
