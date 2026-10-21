namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A legendary card's short name standing for the card itself in an object slot ("put a +1/+1 counter on crovax", "you may exile hikari"): like <see cref="ShortName"/>, but only where the name ends the clause or is followed by a comma, "and" or "for each", so it can't swallow loose words mid-sentence. Excludes the same function words as ShortName, plus roman numerals (saga chapters) and negated auxiliaries. Tried after every other permanent phrase.</summary>
/// <exampledoc>Elenda, the Dusk Rose</exampledoc>
/// <examplecapture>elenda</examplecapture>
[Dependent]
[Referent]
public class ShortNameObject : Glyph
{
    public override Nib[] Nibs => [Pattern(@"(?!(?:a|an|the|this|that|those|these|target|each|all|another|other|it|its|they|them|their|you|your|enchanted|equipped|any|no|x|one|two|three|four|five|six|seven|eight|nine|ten|half|both|either|up|after|if|solve|unsolved|untapped|tapped|attacking|blocking|nontoken|is|are|was|were|has|have|had|can|can't|gets?|gains?|deals?|becomes?|enters?|dies|attacks?|blocks?|loses?|and|or|of|to|from|on|in|with|for|as|at|until|then|where|unless|instead|may|must|would|does|doesn't|don't|isn't|wasn't|weren't|didn't|hasn't|haven't|creatures?|players?|opponents?|permanents?|lands?|cards?|spells?|artifacts?|enchantments?|instants?|sorcery|sorceries|planeswalkers?|tokens?|[ivx]+|[0-9]+)\b)[a-z][a-z'-]*(?: (?!(?:deals|attacks|enters|dies|blocks|becomes|gets|gains|has|can't|is|isn't|was|wasn't|weren't|didn't|and|or|leaves|deal|attack|from|to|on|with|until|at|in|of|for|this|that|it|its|them|their|a|an|the|cards?|creatures?|lands?|permanents?|[ivx]+)\b)[a-z][a-z'-]*)?(?=\s*(?:$|[,.)""]|and\b|for each\b))")];
}
