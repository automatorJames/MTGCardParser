namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Shah of Naar Isle, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Shah of Naar Isle</exampledoc>
[Dependent]
public class ShahOfNaarIsleEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each opponent may draw up to three cards"];
}
