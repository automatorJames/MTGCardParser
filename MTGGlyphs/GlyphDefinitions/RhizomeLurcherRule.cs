namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Rhizome Lurcher, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rhizome Lurcher</exampledoc>
[TokenizationOrder(-1)]
public class RhizomeLurcherRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["undergrowth — this creature enters with a number of +1/+1 counters on it equal to the number of creature cards in your graveyard"];
}
