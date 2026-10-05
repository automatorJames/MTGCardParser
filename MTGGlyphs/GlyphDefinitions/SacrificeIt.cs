namespace MTGGlyphs.GlyphDefinitions;

public class SacrificeIt : Glyph
{
    public override Nib[] Nibs => [Prop(Controller), Plural("sacrifice"), "it"];

    [Optional]
    public ThatCardsController Controller { get; set; }
}
