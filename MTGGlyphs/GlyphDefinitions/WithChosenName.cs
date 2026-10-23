namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A name chosen earlier by the ability, as a qualifier: "with the chosen name", "with the chosen names", "with that name". Used by name-choosing cards (Meddling Mage, Pithing Needle, Conjurer's Ban).</summary>
/// <exampledoc>Meddling Mage</exampledoc>
/// <examplecapture>with the chosen name</examplecapture>
[Dependent]
public class WithChosenName : Glyph
{
    public override Nib[] Nibs => ["with", Prop(Name)];

    public ChosenNameReference Name { get; set; }
}
