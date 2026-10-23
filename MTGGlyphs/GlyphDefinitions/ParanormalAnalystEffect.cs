namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Paranormal Analyst, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Paranormal Analyst</exampledoc>
[Dependent]
public class ParanormalAnalystEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put a card you put into your graveyard this way into your hand"];
}
