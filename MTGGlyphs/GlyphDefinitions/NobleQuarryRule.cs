namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Noble Quarry, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Noble Quarry</exampledoc>
[TokenizationOrder(-1)]
public class NobleQuarryRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["all creatures able to block this creature or enchanted creature do so"];
}
