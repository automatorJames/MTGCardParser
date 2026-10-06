namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Something a targeted player does: "target player draws three cards".</summary>
/// <exampledoc>Ancestral Recall</exampledoc>
/// <examplecapture>target player draws three cards</examplecapture>
public class TargetPlayerAction : Glyph
{
    public override Nib[] Nibs => ["target", Prop(PlayerIdentity), Prop(Action)];

    public PlayerIdentity PlayerIdentity { get; set; }
    public DynamicGlyph Action { get; set; }
}