namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Ojer Axonil, Deepest Might // Temple of Power, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ojer Axonil, Deepest Might // Temple of Power</exampledoc>
/// <examplecapture>if a red source you control would deal an amount of noncombat damage less than ojer axonil's power to an opponent, that source deals damage equal to ojer axonil's power instead</examplecapture>
[TokenizationOrder(-1)]
public class OjerAxonilDeepestMightRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if a red source you control would deal an amount of noncombat damage less than ojer axonil's power to an opponent, that source deals damage equal to ojer axonil's power instead"];
}
