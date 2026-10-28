namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Mountain Stronghold, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mountain Stronghold</exampledoc>
[TokenizationOrder(-1)]
public class MountainStrongholdRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["red legendary creatures you control have \"bands with other legendary creatures"];
}
