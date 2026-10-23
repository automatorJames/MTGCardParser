namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A legendary card's short name just before "'s", standing for the card itself ("syr faren's power", "molimo's power and toughness …"): the corpus replaces only the full name with {this}. Up to four words, the first not an article, pronoun, connective, quantifier, number or a noun a possessive object reads (creature, card, spell, player…). Tried after every other possessive object.</summary>
/// <exampledoc>Syr Faren, the Hengehammer</exampledoc>
/// <examplecapture>syr faren</examplecapture>
[Dependent]
[Referent]
public class PossessedShortName : Glyph
{
    public override Nib[] Nibs => [Pattern(@"\b(?!(?:an?|the|this|that|its|their|your|it|they|you|then|and|or|if|when|whenever|where|as|at|for|of|to|with|until|unless|x|target|each|other|another|enchanted|equipped|defending|creatures?|lands?|tokens?|vehicles?|artifacts?|enchantments?|permanents?|equipment|auras?|spells?|cards?|planeswalkers?|players?|opponents?)\b)[a-z][a-z-]*(?: [a-z][a-z-]*){0,3}(?='s\b)")];
}
