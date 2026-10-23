namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Bladehold War-Whip, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Bladehold War-Whip</exampledoc>
[TokenizationOrder(-1)]
public class BladeholdWarWhipRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["equip abilities you activate of other equipment cost {1} less to activate"];
}
