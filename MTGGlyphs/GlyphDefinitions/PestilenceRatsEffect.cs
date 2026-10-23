namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Pestilence Rats, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Pestilence Rats</exampledoc>
[Dependent]
public class PestilenceRatsEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["for example, as long as there are two other rats on the battlefield,", Nib.This, "'s power and toughness are 2/3"];
}
