namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Lavinia of the Tenth, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Lavinia of the Tenth</exampledoc>
[Dependent]
public class LaviniaOfTheTenthEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["detain each nonland permanent your opponents control with mana value 4 or less"];
}
