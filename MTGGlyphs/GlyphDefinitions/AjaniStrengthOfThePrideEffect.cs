namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Ajani, Strength of the Pride, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ajani, Strength of the Pride</exampledoc>
[Dependent]
public class AjaniStrengthOfThePrideEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you have at least 15 life more than your starting life total, exile ajani and each artifact and creature your opponents control"];
}
