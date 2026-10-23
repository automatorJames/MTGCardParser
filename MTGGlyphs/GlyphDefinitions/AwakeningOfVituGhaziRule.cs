namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Awakening of Vitu-Ghazi, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Awakening of Vitu-Ghazi</exampledoc>
[TokenizationOrder(-1)]
public class AwakeningOfVituGhaziRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it becomes a legendary 0/0 elemental creature with haste named vitu-ghazi"];
}
