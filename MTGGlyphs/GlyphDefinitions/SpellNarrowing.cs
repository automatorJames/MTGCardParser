namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What narrows a spell after its noun: what it targets ("that targets this creature"), its colors ("that's both red and white", "that's white, blue, black, or red"), its chosen color or type ("of the chosen color"), where it's cast from ("from your graveyard", "from anywhere other than your hand"), when ("during an opponent's turn"), a keyword or feature it has ("that has flash", "that has an adventure"), a symbol in its mana cost ("with {x} in its mana cost"), or that its caster doesn't own it ("you don't own").</summary>
/// <exampledoc>Heroes of the Revel</exampledoc>
/// <examplecapture>that targets this creature</examplecapture>
[Dependent]
public class SpellNarrowing : GlyphOneOf
{
    public SpellTargets Targets { get; set; }
    public SpellColors Colors { get; set; }
    public ChosenAspect? Chosen { get; set; }
    public FromZone From { get; set; }
    public CastFromOtherThanHand FromOtherThanHand { get; set; }
    public DuringTurn During { get; set; }
    public SpellHasFeature Feature { get; set; }
    public SymbolInManaCost Symbol { get; set; }
    public SpellOwnership? Ownership { get; set; }
}
