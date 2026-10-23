namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Ignis Scientia, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ignis Scientia</exampledoc>
[Dependent]
public class IgnisScientiaEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile target card from a graveyard"];
}
