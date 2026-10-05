namespace MTGGlyphs.GlyphDefinitions;

public class DestroyAll : Glyph
{
    public override Nib[] Nibs => ["destroy all", Prop(Destroyed)];

    public CreaturesBlockingOrBlockedBy Destroyed { get; set; }
}
