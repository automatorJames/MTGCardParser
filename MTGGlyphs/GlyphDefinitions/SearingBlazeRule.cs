namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Searing Blaze, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Searing Blaze</exampledoc>
/// <examplecapture>landfall — if you had a land enter the battlefield under your control this turn, {this} deals 3 damage to that player or planeswalker and 3 damage to that creature instead</examplecapture>
[TokenizationOrder(-1)]
public class SearingBlazeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["landfall — if you had a land enter the battlefield under your control this turn,", Nib.This, "deals 3 damage to that player or planeswalker and 3 damage to that creature instead"];
}
