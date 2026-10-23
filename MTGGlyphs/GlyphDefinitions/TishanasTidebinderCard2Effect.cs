namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Tishana's Tidebinder, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tishana's Tidebinder</exampledoc>
[Dependent]
public class TishanasTidebinderCard2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["counter up to one target activated or triggered ability"];
}
