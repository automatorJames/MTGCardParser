namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Fatespinner, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Fatespinner</exampledoc>
[Dependent]
public class FatespinnerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["that player chooses draw step, main phase, or combat phase"];
}
