namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Angelic Skirmisher, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Angelic Skirmisher</exampledoc>
[Dependent]
public class AngelicSkirmisherEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose first strike, vigilance, or lifelink"];
}
