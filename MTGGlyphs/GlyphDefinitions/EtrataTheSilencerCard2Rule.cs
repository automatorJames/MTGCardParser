namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Etrata, the Silencer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Etrata, the Silencer</exampledoc>
[TokenizationOrder(-1)]
public class EtrataTheSilencerCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["etrata's owner shuffles etrata into their library"];
}
