namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"banded with it", "banded with this creature": creatures in a band with a creature named earlier (banding).</summary>
/// <exampledoc>Icatian Skirmishers</exampledoc>
/// <examplecapture>banded with it</examplecapture>
[Dependent]
public class BandedWith : Glyph
{
    public override Nib[] Nibs => ["banded with", Prop(Partner)];

    public OneOf<It, ThisCreature> Partner { get; set; }
}
