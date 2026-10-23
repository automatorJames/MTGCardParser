namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Nick Fury, Agent of S.H.I.E.L.D., held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Nick Fury, Agent of S.H.I.E.L.D.</exampledoc>
[TokenizationOrder(-1)]
public class NickFuryAgentOfSHIELDRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it's a double-faced card, you may transform it"];
}
