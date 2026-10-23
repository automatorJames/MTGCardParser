namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Wolverine, Fierce Fighter, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Wolverine, Fierce Fighter</exampledoc>
[TokenizationOrder(-1)]
public class WolverineFierceFighterRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if damage would be dealt to wolverine, instead that damage is dealt, but all other damage already dealt to him is healed"];
}
