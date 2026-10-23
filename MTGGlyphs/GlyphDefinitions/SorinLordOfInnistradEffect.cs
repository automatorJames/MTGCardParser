namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Sorin, Lord of Innistrad, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sorin, Lord of Innistrad</exampledoc>
[Dependent]
public class SorinLordOfInnistradEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy up to three target creatures and/or other planeswalkers"];
}
