namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Stormscale Anarch, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Stormscale Anarch</exampledoc>
[TokenizationOrder(-1)]
public class StormscaleAnarchRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if the discarded card was multicolored, this creature deals 4 damage instead"];
}
