namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Ashiok, Nightmare Muse, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ashiok, Nightmare Muse</exampledoc>
[Dependent]
public class AshiokNightmareMuseEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may cast up to three spells from among face-up cards your opponents own from exile without paying their mana costs"];
}
