namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Kylox, Visionary Inventor, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Kylox, Visionary Inventor</exampledoc>
[Dependent]
public class KyloxVisionaryInventorEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["sacrifice any number of other creatures, then exile the top x cards of your library, where x is their total power"];
}
