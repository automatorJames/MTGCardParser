namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"then that player shuffles": the tail of a search effect. Now also parsed compositionally (ThenEffect + SubjectDoes + ShuffleLibrary); kept first so its lines don't move until the person approves removing it.</summary>
/// <exampledoc>Ghost Quarter</exampledoc>
/// <examplecapture>then that player shuffles</examplecapture>
[TokenizationOrder(0)]
public class ThenThatPlayerShuffles : Glyph
{
    public override Nib[] Nibs => ["then that player shuffles"];
}
