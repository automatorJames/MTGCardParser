namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Amass's fallback: "if you don't control an army, create a 0/0 black zombie army creature token first" - the token to create before putting counters on it.</summary>
/// <exampledoc>Lazotep Reaver</exampledoc>
/// <examplecapture>if you don't control an army, create a 0/0 black zombie army creature token first</examplecapture>
public class CreateFirstIfNone : Glyph
{
    public override Nib[] Nibs => ["if you don't control", Prop(Missing), ", create", Prop(Token), "first"];

    public IndefinitePermanent Missing { get; set; }
    public TokenDescription Token { get; set; }
}
