namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Yenna, Redtooth Regent, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Yenna, Redtooth Regent</exampledoc>
[Dependent]
public class YennaRedtoothRegentEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose target enchantment you control that doesn't have the same name as another permanent you control"];
}
