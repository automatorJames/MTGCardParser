namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A permanent having several extra creature types: "is also a cleric, rogue, warrior, and wizard" (the party-type cards). Kept apart from IsOfType, whose list reading would take "is a mutant, ninja, or turtle" conditions.</summary>
/// <exampledoc>Tajuru Paragon</exampledoc>
/// <examplecapture>is also a cleric, rogue, warrior, and wizard</examplecapture>
[Dependent]
public class IsAlsoTypes : Glyph, IPredicate
{
    public override Nib[] Nibs => [Pattern("is also an?"), Prop(Types)];

    public ManyOf<CreatureType> Types { get; set; }
}
