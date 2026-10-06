namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"destroy all creatures blocking or blocked by it".</summary>
public class DestroyAll : Glyph
{
    public override Nib[] Nibs => ["destroy all", Prop(Destroyed)];

    public CreaturesBlockingOrBlockedBy Destroyed { get; set; }
}
