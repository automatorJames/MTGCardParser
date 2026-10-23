namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Devastating Mastery, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Devastating Mastery</exampledoc>
[TokenizationOrder(-1)]
public class DevastatingMasteryRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if the {2}{w}{w} cost was paid, an opponent chooses up to two nonland permanents they control and returns them to their owner's hand"];
}
