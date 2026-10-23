namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Comet Storm, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Comet Storm</exampledoc>
[TokenizationOrder(-1)]
public class CometStormRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose any target, then choose another target for each time this spell was kicked"];
}
