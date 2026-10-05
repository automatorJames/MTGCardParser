namespace MTGGlyphs.GlyphDefinitions;

public class DestroyAll : Glyph
{
    public override Nib[] Nibs => ["destroy all", Prop(Permanents)];

    public OneOf<ManyOf<CardType>, CardType?, LandType?> Permanents { get; set; }
}
