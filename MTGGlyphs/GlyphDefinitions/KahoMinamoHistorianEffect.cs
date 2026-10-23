namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Kaho, Minamo Historian, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Kaho, Minamo Historian</exampledoc>
[Dependent]
public class KahoMinamoHistorianEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may cast a spell with mana value x from among cards exiled with kaho without paying its mana cost"];
}
