namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Shipwreck Sifters, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Shipwreck Sifters</exampledoc>
[Dependent]
public class ShipwreckSiftersEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put a +1/+1 counter on this creature"];
}
