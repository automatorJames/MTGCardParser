namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Zurgo, Thunder's Decree, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Zurgo, Thunder's Decree</exampledoc>
[TokenizationOrder(-1)]
public class ZurgoThundersDecreeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["during your end step, warrior tokens you control have \"this token can't be sacrificed"];
}
