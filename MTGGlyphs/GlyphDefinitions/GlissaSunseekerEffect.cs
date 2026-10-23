namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Glissa Sunseeker, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Glissa Sunseeker</exampledoc>
[Dependent]
public class GlissaSunseekerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy target artifact if its mana value is equal to the amount of unspent mana you have"];
}
