namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A spell that can't be countered or copied: "this spell can't be countered", "the next creature spell you cast this turn can't be countered", "this spell can't be copied".</summary>
/// <exampledoc>Abrupt Decay</exampledoc>
/// <examplecapture>this spell can't be countered</examplecapture>
public class CantBeCountered : Glyph
{
    public override Nib[] Nibs => [Prop(Spell), Alt("can't be countered", "can't be copied")];

    public OneOf<ThisSpell, OrdinalSpellCast> Spell { get; set; }
}
