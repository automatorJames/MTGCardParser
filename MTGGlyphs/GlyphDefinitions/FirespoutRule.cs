namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Firespout, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Firespout</exampledoc>
/// <examplecapture>{this} deals 3 damage to each creature without flying if {r} was spent to cast this spell and 3 damage to each creature with flying if {g} was spent to cast this spell</examplecapture>
[TokenizationOrder(-1)]
public class FirespoutRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => [Nib.This, "deals 3 damage to each creature without flying if {r} was spent to cast this spell and 3 damage to each creature with flying if {g} was spent to cast this spell"];
}
