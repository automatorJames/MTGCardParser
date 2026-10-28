namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Stronghold Gambit, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Stronghold Gambit</exampledoc>
[TokenizationOrder(-1)]
public class StrongholdGambitRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then each player reveals their chosen card"];
}
