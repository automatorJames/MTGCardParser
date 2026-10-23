namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Ouphe Vandals, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ouphe Vandals</exampledoc>
[Dependent]
public class OupheVandalsEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["counter target activated ability from an artifact source and destroy that artifact if it's on the battlefield"];
}
