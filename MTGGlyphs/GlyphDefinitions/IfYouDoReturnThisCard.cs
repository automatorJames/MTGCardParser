namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"if you do, return this card from your graveyard to your hand" (dredge, recover). Tried first, so IfOutcome with ReturnCard doesn't take it; the two could merge.</summary>
/// <exampledoc>Recurring Nightmare</exampledoc>
/// <examplecapture>if you do, return this card from your graveyard to your hand</examplecapture>
[TokenizationOrder(0)]
public class IfYouDoReturnThisCard : Glyph
{
    public override Nib[] Nibs => ["if you do, return this card from your graveyard to your hand"];
}
