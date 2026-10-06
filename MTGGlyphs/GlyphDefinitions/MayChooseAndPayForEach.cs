namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"that player may choose any number of tapped blue creatures they control and pay {4} for each creature chosen this way".</summary>
public class MayChooseAndPayForEach : Glyph
{
    public override Nib[] Nibs => [Prop(Chooser), "may choose any number of", Prop(Chosen), "and pay", Prop(Cost), "for each", Prop(ChosenType), "chosen this way"];

    public ThatPlayer Chooser { get; set; }
    public PermanentsTheyControl Chosen { get; set; }
    public ManaValue Cost { get; set; }
    public CardType ChosenType { get; set; }
}
