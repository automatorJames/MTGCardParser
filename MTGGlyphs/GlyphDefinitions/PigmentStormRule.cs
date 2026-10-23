namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Pigment Storm, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Pigment Storm</exampledoc>
[TokenizationOrder(-1)]
public class PigmentStormRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["excess damage is dealt to that creature's controller instead"];
}
