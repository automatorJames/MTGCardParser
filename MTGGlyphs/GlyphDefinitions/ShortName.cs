namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A legendary card's short name standing for the card itself as a subject ("whenever crosis deals combat damage…"): Oracle text names legends by the part of the name before the comma or epithet, which the corpus doesn't replace with {this}. One or two words (accented letters included: "dáin", "mjölnir"), optionally after "the" ("the gitrog", "the mighty thor"), that aren't an article, pronoun, number, quantifier, card type, preposition or common verb; offered only as a subject, after every other one, since in a bare object slot it would match any loose words.</summary>
/// <exampledoc>Crosis, the Purger</exampledoc>
/// <examplecapture>crosis</examplecapture>
[Dependent]
[Referent]
public class ShortName : Glyph
{
    public override Nib[] Nibs => [Pattern(@"(?:the )?(?!(?:a|an|the|this|that|those|these|target|each|all|another|other|it|its|they|them|their|you|your|enchanted|equipped|any|no|x|one|two|three|four|five|six|seven|eight|nine|ten|half|both|either|up|after|if|unsolved|untapped|tapped|attacking|blocking|nontoken|is|are|was|were|has|have|had|can|can't|gets?|gains?|deals?|becomes?|enters?|dies|attacks?|blocks?|loses?|and|or|of|to|from|on|in|with|for|as|at|until|then|where|unless|instead|may|must|would|does|doesn't|don't|isn't|creatures?|players?|opponents?|permanents?|lands?|cards?|spells?|artifacts?|enchantments?|instants?|sorcery|sorceries|planeswalkers?|tokens?|[0-9]+)\b)\p{Ll}[\p{Ll}'-]*(?: (?!(?:deals|attacks|enters|dies|blocks|becomes|gets|gains|has|can't|is|and|or|leaves|deal|attack|from|to|on|with|until|at|in|of|this|that|it|its|them|their|a|an|the|cards?|creatures?|lands?|permanents?|players?|spells?|artifacts?|enchantments?|planeswalkers?|tokens?|was|were)\b)\p{Ll}[\p{Ll}'-]*)?")];
}
