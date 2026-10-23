namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Brine Comber // Brinebound Gift, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Brine Comber // Brinebound Gift</exampledoc>
[Dependent]
public class BrineComberEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create a 1/1 white spirit creature token with flying"];
}
