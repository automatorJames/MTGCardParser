namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Akron Legionnaire, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Akron Legionnaire</exampledoc>
[TokenizationOrder(-1)]
public class AkronLegionnaireRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["except for creatures named", Nib.This, "and artifact creatures, creatures you control can't attack"];
}
