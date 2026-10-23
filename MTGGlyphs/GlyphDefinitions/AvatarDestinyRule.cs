namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Avatar Destiny, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Avatar Destiny</exampledoc>
[TokenizationOrder(-1)]
public class AvatarDestinyRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["return this card to its owner's hand and up to one creature card milled this way to the battlefield under your control"];
}
