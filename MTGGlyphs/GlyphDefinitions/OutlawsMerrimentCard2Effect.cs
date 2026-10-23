namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Outlaws' Merriment, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Outlaws' Merriment</exampledoc>
[Dependent]
public class OutlawsMerrimentCard2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose one at random"];
}
