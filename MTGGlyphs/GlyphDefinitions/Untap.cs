namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"untap those creatures".</summary>
public class Untap : Glyph
{
    public override Nib[] Nibs => ["untap", Prop(Untapped)];

    public ThatCard Untapped { get; set; }
}
