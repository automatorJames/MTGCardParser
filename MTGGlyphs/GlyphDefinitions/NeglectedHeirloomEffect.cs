namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Neglected Heirloom // Ashmouth Blade, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Neglected Heirloom // Ashmouth Blade</exampledoc>
[Dependent]
public class NeglectedHeirloomEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["transform this equipment"];
}
