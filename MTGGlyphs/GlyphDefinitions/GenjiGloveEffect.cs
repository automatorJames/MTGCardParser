namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Genji Glove, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Genji Glove</exampledoc>
[Dependent]
public class GenjiGloveEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it's the first combat phase of the turn, untap it"];
}
