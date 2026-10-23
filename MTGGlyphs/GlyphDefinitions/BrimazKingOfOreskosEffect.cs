namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Brimaz, King of Oreskos, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Brimaz, King of Oreskos</exampledoc>
[Dependent]
public class BrimazKingOfOreskosEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create a 1/1 white cat soldier creature token with vigilance that's blocking that creature"];
}
