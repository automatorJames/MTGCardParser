namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Steel Hellkite, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Steel Hellkite</exampledoc>
[Dependent]
public class SteelHellkiteEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy each nonland permanent with mana value x whose controller was dealt combat damage by this creature this turn"];
}
