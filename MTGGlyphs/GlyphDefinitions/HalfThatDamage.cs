namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Half the damage a replacement condition just named, with its rounding: "half that damage, rounded down". The comma is a literal here, since a nested glyph can't open with one.</summary>
/// <exampledoc>Ghosts of the Innocent</exampledoc>
/// <examplecapture>half that damage, rounded down</examplecapture>
[Dependent]
public class HalfThatDamage : Glyph
{
    public override Nib[] Nibs => ["half that damage,", Prop(Rounded)];

    public Rounding Rounded { get; set; }
}
