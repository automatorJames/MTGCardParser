namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Aurelia's Vindicator, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Aurelia's Vindicator</exampledoc>
[Dependent]
public class AureliasVindicatorEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile up to x other target creatures from the battlefield and/or creature cards from graveyards"];
}
