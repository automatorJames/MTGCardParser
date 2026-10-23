namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Danitha, Sword of Hope, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Danitha, Sword of Hope</exampledoc>
[Dependent]
public class DanithaSwordOfHopeEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["draw a card"];
}
