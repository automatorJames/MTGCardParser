namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Brine Comber // Brinebound Gift, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Brine Comber // Brinebound Gift</exampledoc>
[TokenizationOrder(-1)]
public class BrineComberRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever this creature enters or becomes the target of an aura spell, create a 1/1 white spirit creature token with flying"];
}
