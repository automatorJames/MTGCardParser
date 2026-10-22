namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A replacement exiling a permanent instead of letting it die or leave the battlefield another way: "if that creature would die this turn, exile it instead", "if a creature an opponent controls would die, exile it instead", "if a creature dealt damage this way would die this turn, exile it instead", "if it would leave the battlefield, exile it instead of putting it anywhere else", "if those auras would leave the battlefield, exile them instead of putting them anywhere else".</summary>
/// <exampledoc>Puncturing Blow</exampledoc>
/// <examplecapture>if that creature would die this turn, exile it instead</examplecapture>
public class DieReplacement : Glyph
{
    public override Nib[] Nibs => ["if", Prop(Dying), "would", Prop(Event), Prop(Period), ", exile", Alt("it", "them"), "instead", Prop(Anywhere)];

    public OneOf<DamagedPermanent, PermanentPhrase, IndefinitePermanent> Dying { get; set; }
    public DepartureEvent Event { get; set; }
    [Optional]
    public ThisPeriod Period { get; set; }
    [RegexPattern("of putting (?:it|them) anywhere else")]
    public bool Anywhere { get; set; }
}
