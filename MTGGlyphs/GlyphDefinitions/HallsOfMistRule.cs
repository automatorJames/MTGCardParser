namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Halls of Mist, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Halls of Mist</exampledoc>
[TokenizationOrder(-1)]
public class HallsOfMistRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["creatures that attacked during their controller's last turn can't attack"];
}
