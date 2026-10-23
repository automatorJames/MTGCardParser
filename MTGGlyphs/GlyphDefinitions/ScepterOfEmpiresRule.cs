namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Scepter of Empires, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Scepter of Empires</exampledoc>
[TokenizationOrder(-1)]
public class ScepterOfEmpiresRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it deals 3 damage instead if you control artifacts named crown of empires and throne of empires"];
}
