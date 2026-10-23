namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Heart of Bogardan, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Heart of Bogardan</exampledoc>
/// <examplecapture>this enchantment deals x damage to target player or planeswalker and each creature that player or that planeswalker's controller controls, where x is twice the number of age counters on this enchantment minus 2</examplecapture>
[Dependent]
public class HeartOfBogardanEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this enchantment deals x damage to target player or planeswalker and each creature that player or that planeswalker's controller controls, where x is twice the number of age counters on this enchantment minus 2"];
}
