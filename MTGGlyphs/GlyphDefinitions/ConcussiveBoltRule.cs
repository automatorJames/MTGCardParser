namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Concussive Bolt, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Concussive Bolt</exampledoc>
[TokenizationOrder(-1)]
public class ConcussiveBoltRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["metalcraft — if you control three or more artifacts, creatures controlled by that player or by that planeswalker's controller can't block this turn"];
}
