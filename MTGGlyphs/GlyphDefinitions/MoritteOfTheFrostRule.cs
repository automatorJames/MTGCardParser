namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Moritte of the Frost, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Moritte of the Frost</exampledoc>
/// <examplecapture>you may have moritte enter as a copy of a permanent you control, except it's legendary and snow in addition to its other types and, if it's a creature, it enters with two additional +1/+1 counters on it and has changeling</examplecapture>
[TokenizationOrder(-1)]
public class MoritteOfTheFrostRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may have moritte enter as a copy of a permanent you control, except it's legendary and snow in addition to its other types and, if it's a creature, it enters with two additional +1/+1 counters on it and has changeling"];
}
