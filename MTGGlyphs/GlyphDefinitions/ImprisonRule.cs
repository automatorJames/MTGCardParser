namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Imprison, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Imprison</exampledoc>
/// <examplecapture>if you do, tap the creature, remove it from combat, and creatures it was blocking that had become blocked by only that creature this combat become unblocked</examplecapture>
[TokenizationOrder(-1)]
public class ImprisonRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you do, tap the creature, remove it from combat, and creatures it was blocking that had become blocked by only that creature this combat become unblocked"];
}
