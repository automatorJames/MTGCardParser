namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Boiling Rock Rioter, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Boiling Rock Rioter</exampledoc>
[Dependent]
public class BoilingRockRioterEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may cast an ally spell from among cards you own exiled with this creature"];
}
