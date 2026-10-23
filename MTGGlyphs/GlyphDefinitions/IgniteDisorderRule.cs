namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Ignite Disorder, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ignite Disorder</exampledoc>
[TokenizationOrder(-1)]
public class IgniteDisorderRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => [Nib.This, "deals 3 damage divided as you choose among one, two, or three target white and/or blue creatures"];
}
