namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A whole effect or rule under a leading duration, where it isn't a subject and predicate (those are DurationFirstEffect): "until end of turn, if a creature would enter and it wasn't cast, exile it instead", "until end of turn, whenever a player gains life, …", "until end of turn, damage that would reduce your life total to less than 1 reduces it to 1 instead", "for as long as that card remains exiled, its owner may play it". Tried after other glyphs, so DurationFirstEffect, WhereXIs and sequence frames keep their lines.</summary>
/// <exampledoc>Hallowed Moonlight</exampledoc>
/// <examplecapture>until end of turn, if a creature would enter and it wasn't cast, exile it instead</examplecapture>
[TokenizationOrder(-2)]
public class DurationFirstRule : Glyph
{
    public override Nib[] Nibs => [Prop(Duration), ",", Prop(Effect)];

    public OneOf<UntilPhase, UntilEndOfPlayersTurn, DuringTurn, ForTheRestOfTheGame, ForAsLongAs> Duration { get; set; }
    public DynamicGlyph Effect { get; set; }
}
