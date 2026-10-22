namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"named ajani, valiant protector": a card or permanent picked out by another card's name, spelled out. Names can hold commas, so the name runs word by word until a word that starts what follows it ("reveal", "put", "and", "then", "from", "into", "onto", "to").</summary>
/// <exampledoc>Ajani's Aid</exampledoc>
/// <examplecapture>named ajani, valiant protector</examplecapture>
[Dependent]
public class NamedCard : Glyph
{
    public override Nib[] Nibs => ["named", Pattern(@"(?!(?:reveals?|puts?|and|then|from|into|onto|to)\b)[a-z0-9'\-]+(?:,? (?!(?:reveals?|puts?|and|then|from|into|onto|to)\b)[a-z0-9'\-]+)*")];
}
