namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Goldspan Dragon, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Goldspan Dragon</exampledoc>
[Dependent]
public class GoldspanDragonEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create a treasure token"];
}
