namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Innocent Bystander, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Innocent Bystander</exampledoc>
[TokenizationOrder(-1)]
public class InnocentBystanderRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever this creature is dealt 3 or more damage, investigate"];
}
