namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Balduvian Warlord, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Balduvian Warlord</exampledoc>
[TokenizationOrder(-1)]
public class BalduvianWarlordRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["creatures it was blocking that hadn't become blocked by another creature this combat become unblocked, then it blocks an attacking creature of your choice"];
}
