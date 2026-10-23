namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Rain of Gore, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rain of Gore</exampledoc>
[TokenizationOrder(-1)]
public class RainOfGoreRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if a spell or ability would cause its controller to gain life, that player loses that much life instead"];
}
