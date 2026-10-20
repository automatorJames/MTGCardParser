namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A replacement exiling a permanent instead of letting it die: "if that creature would die this turn, exile it instead", "if a creature an opponent controls would die, exile it instead", "if a creature dealt damage this way would die this turn, exile it instead".</summary>
/// <exampledoc>Puncturing Blow</exampledoc>
/// <examplecapture>if that creature would die this turn, exile it instead</examplecapture>
public class DieReplacement : Glyph
{
    public override Nib[] Nibs => ["if", Prop(Dying), "would die", Prop(Period), ", exile it instead"];

    public OneOf<DamagedPermanent, PermanentPhrase, IndefinitePermanent> Dying { get; set; }
    [Optional]
    public ThisPeriod Period { get; set; }
}
