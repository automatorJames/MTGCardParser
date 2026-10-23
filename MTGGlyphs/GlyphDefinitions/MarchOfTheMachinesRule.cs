namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of March of the Machines, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>March of the Machines</exampledoc>
[TokenizationOrder(-1)]
public class MarchOfTheMachinesRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each noncreature artifact is an artifact creature with power and toughness each equal to its mana value"];
}
