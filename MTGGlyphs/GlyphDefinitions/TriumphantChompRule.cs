namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Triumphant Chomp, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Triumphant Chomp</exampledoc>
[TokenizationOrder(-1)]
public class TriumphantChompRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => [Nib.This, "deals damage to target creature equal to 2 or the greatest power among dinosaurs you control, whichever is greater"];
}
