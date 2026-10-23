namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Cellar Door, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Cellar Door</exampledoc>
[Dependent]
public class CellarDoorEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target player puts the bottom card of their library into their graveyard"];
}
