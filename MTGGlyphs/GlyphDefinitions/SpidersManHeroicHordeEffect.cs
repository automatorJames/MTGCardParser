namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Spiders-Man, Heroic Horde, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Spiders-Man, Heroic Horde</exampledoc>
[Dependent]
public class SpidersManHeroicHordeEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if they were cast using web-slinging, you gain 3 life and create two 2/1 green spider creature tokens with reach"];
}
