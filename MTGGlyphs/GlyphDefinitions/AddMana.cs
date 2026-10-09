namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A mana ability's effect: "add {g}", "add {c}{c}{c}", "add {b} or {r}", "add one mana of any color".</summary>
/// <exampledoc>Llanowar Elves</exampledoc>
/// <examplecapture>add {g}</examplecapture>
public class AddMana : Glyph
{
    public override Nib[] Nibs => ["add", Prop(Mana)];

    public OneOf<ManyOf<ManaCost>, ManaCost, ManaOfAnyColor> Mana { get; set; }
}
