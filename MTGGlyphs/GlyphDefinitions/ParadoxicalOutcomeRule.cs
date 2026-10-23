namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Paradoxical Outcome, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Paradoxical Outcome</exampledoc>
[TokenizationOrder(-1)]
public class ParadoxicalOutcomeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["return any number of target nonland, nontoken permanents you control to their owners' hands"];
}
