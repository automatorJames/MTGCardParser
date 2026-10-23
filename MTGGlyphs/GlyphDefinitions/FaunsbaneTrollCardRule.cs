namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Faunsbane Troll, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Faunsbane Troll</exampledoc>
[TokenizationOrder(-1)]
public class FaunsbaneTrollCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["{1}, sacrifice an aura attached to this creature: this creature fights target creature you don't control"];
}
