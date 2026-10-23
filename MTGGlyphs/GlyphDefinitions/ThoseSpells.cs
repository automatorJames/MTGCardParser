namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"those spells": spells of a kind named earlier in the line ("as an additional cost to cast red permanent spells, … those spells cost {r} less to cast …").</summary>
/// <exampledoc>Defiler of Vigor</exampledoc>
/// <examplecapture>those spells</examplecapture>
[Dependent]
[RegexPattern("those spells")]
public class ThoseSpells : BackReference<SpellsCast>;
