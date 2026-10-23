namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Tezzeret, Betrayer of Flesh, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tezzeret, Betrayer of Flesh</exampledoc>
[TokenizationOrder(-1)]
public class TezzeretBetrayerOfFleshRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the first activated ability of an artifact you activate each turn costs {2} less to activate"];
}
