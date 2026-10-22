namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"white spells and black spells you cast", "goblin spells and rogue spells you cast": spells of either of several kinds, each named with its own "spells" (the Familiars and Bannerets).</summary>
/// <exampledoc>Stormscape Familiar</exampledoc>
/// <examplecapture>white spells and black spells you cast</examplecapture>
[Dependent]
public class SpellKindsCast : Glyph
{
    public override Nib[] Nibs => [Prop(Kinds), Prop(Caster)];

    public ManyOf<KindOfSpells> Kinds { get; set; }
    [Optional]
    public SpellCaster Caster { get; set; }
}
