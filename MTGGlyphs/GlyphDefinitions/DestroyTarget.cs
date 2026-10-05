namespace MTGGlyphs.GlyphDefinitions;

public class DestroyTarget : Glyph
{
    public override Nib[] Nibs => ["destroy", Prop(Destroyed)];

    public TargetPermanent Destroyed { get; set; }
}
