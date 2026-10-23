namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Medomai the Ageless, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Medomai the Ageless</exampledoc>
[TokenizationOrder(-1)]
public class MedomaiTheAgelessRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["medomai can't attack during extra turns"];
}
