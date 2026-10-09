namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Banding's attack rule, from its reminder text: "any creatures with banding, and up to one without, can attack in a band".</summary>
/// <exampledoc>Benalish Hero</exampledoc>
/// <examplecapture>any creatures with banding, and up to one without, can attack in a band</examplecapture>
public class BandingAttackRule : Glyph
{
    public override Nib[] Nibs => ["any creatures with banding, and up to one without, can attack in a band"];
}
