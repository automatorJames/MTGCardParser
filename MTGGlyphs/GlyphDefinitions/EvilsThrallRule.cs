namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Evil's Thrall, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Evil's Thrall</exampledoc>
/// <examplecapture>if you control a villain with greater mana value than that creature, gain control of that creature until the end of your next turn instead</examplecapture>
[TokenizationOrder(-1)]
public class EvilsThrallRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you control a villain with greater mana value than that creature, gain control of that creature until the end of your next turn instead"];
}
