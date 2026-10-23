namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Samite Elder, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Samite Elder</exampledoc>
[TokenizationOrder(-1)]
public class SamiteElderRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["creatures you control gain protection from each of that permanent's colors until end of turn"];
}
