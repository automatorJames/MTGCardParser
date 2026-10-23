namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Mikey & Don, Party Planners, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mikey &amp; Don, Party Planners</exampledoc>
[TokenizationOrder(-1)]
public class MikeyDonPartyPlannersRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you cast a creature spell this way, that creature enters with an additional +1/+1 counter on it"];
}
