namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A legendary card's short name in the possessive, standing for the card itself ("molimo's power and toughness …", "sage of ancient lore's …"): the corpus replaces only the full name with {this}. Up to four words before "'s", not opening with an article, pronoun or a noun "this …" takes (creature, token, vehicle…), since a predicate slot sees the text after "this"; offered only where a possessive opens a power/toughness ability, after every other possessive.</summary>
/// <exampledoc>Molimo, Maro-Sorcerer</exampledoc>
/// <examplecapture>molimo's</examplecapture>
[Dependent]
[Referent]
public class ShortNamePossessive : Glyph
{
    public override Nib[] Nibs => [Pattern(@"\b(?!(?:the|this|that|its|your|creatures?|lands?|tokens?|vehicles?|artifacts?|enchantments?|permanents?|equipment|auras?|spells?|cards?|planeswalkers?)\b)[a-z][a-z-]*(?: [a-z][a-z-]*){0,3}"), "'s"];
}
