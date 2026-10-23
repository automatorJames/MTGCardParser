namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Etrata, the Silencer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Etrata, the Silencer</exampledoc>
[TokenizationOrder(-1)]
public class EtrataTheSilencerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["that player loses the game if they own three or more exiled cards with hit counters on them"];
}
