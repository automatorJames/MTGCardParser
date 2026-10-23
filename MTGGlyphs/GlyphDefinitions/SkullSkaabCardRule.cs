namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Skull Skaab, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Skull Skaab</exampledoc>
[TokenizationOrder(-1)]
public class SkullSkaabCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever a creature you control exploits a nontoken creature, create a 2/2 black zombie creature token"];
}
