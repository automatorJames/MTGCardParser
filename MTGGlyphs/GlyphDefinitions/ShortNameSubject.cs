namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A legend's short name as the subject of an effect or static ability ("{r}: akroma gets +1/+0 until end of turn", "crovax gains flying", "boseiju enters tapped"): <see cref="ShortName"/>'s word rules, but only directly before a predicate verb (gets, gains, has, deals, can't, can, becomes, loses, attacks, blocks, is, must, doesn't, enters, fights, explores), so it can't eat the opening words of other clauses.</summary>
/// <exampledoc>Akroma, Angel of Fury</exampledoc>
/// <examplecapture>akroma</examplecapture>
[Dependent]
[Referent]
public class ShortNameSubject : Glyph
{
    public override Nib[] Nibs => [Pattern(@"(?!(?:a|an|the|this|that|those|these|target|each|all|another|other|it|its|they|them|their|you|your|enchanted|equipped|any|no|x|one|two|three|four|five|six|seven|eight|nine|ten|half|both|either|up|after|if|then|where|unless|instead|creatures?|players?|opponents?|permanents?|lands?|cards?|spells?|artifacts?|enchantments?|instants?|sorcery|sorceries|planeswalkers?|tokens?|[ivx]+|[0-9]+)\b)[a-z][a-z'-]*(?: (?!(?:gets|gains|has|deals|can't|can|becomes|loses|attacks|blocks|is|must|doesn't|enters|fights|explores|and|or|this|that|it|its|a|an|the)\b)[a-z][a-z'-]*)?(?= (?:gets|gains|has|deals|can't|can|becomes|loses|attacks|blocks|is|must|doesn't|enters|fights|explores)\b)")];
}
