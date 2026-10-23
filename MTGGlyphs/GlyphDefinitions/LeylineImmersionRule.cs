namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Leyline Immersion, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Leyline Immersion</exampledoc>
[TokenizationOrder(-1)]
public class LeylineImmersionRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["enchanted creature has ward {2} and \"{t}: add five mana in any combination of colors"];
}
