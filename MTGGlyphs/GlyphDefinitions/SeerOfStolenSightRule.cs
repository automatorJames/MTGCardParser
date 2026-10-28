namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Seer of Stolen Sight, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Seer of Stolen Sight</exampledoc>
[TokenizationOrder(-1)]
public class SeerOfStolenSightRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever one or more artifacts and/or creatures you control are put into a graveyard from the battlefield, surveil 1"];
}
