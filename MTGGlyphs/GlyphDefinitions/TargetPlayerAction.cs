namespace MTGGlyphs.GlyphDefinitions;

public class TargetPlayerAction : Glyph
{
    public override Nib[] Nibs => ["target", Prop(PlayerIdentity), Prop(Action)];

    public PlayerIdentity PlayerIdentity { get; set; }
    public DynamicGlyph Action { get; set; }
}