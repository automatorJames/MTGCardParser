namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Shatterskull Smashing // Shatterskull, the Hammer Pass, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Shatterskull Smashing // Shatterskull, the Hammer Pass</exampledoc>
[TokenizationOrder(-1)]
public class ShatterskullSmashingRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if x is 6 or more, shatterskull smashing deals twice x damage divided as you choose among them instead"];
}
