namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Ral's Staticaster, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ral's Staticaster</exampledoc>
[Dependent]
public class RalsStaticasterEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you control a ral planeswalker, this creature gets +1/+0 for each card in your hand until end of turn"];
}
