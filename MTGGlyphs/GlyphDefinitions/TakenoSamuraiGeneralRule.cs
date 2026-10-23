namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Takeno, Samurai General, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Takeno, Samurai General</exampledoc>
[TokenizationOrder(-1)]
public class TakenoSamuraiGeneralRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each other samurai creature you control gets +1/+1 for each point of bushido it has"];
}
