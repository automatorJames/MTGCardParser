namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"that land's controller", "that creature's controller".</summary>
[Dependent]
public class ThatCardsController : Glyph
{
    public override Nib[] Nibs => ["that", Prop(CardOrCreatureType), "'s controller"];

    public CardOrCreatureType CardOrCreatureType { get; set; }
}
