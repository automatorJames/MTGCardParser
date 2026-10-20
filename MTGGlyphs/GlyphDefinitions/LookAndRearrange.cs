namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Looking at the top of a library and then rearranging it: "look at the top four cards of your library, then put them back in any order", and scry/surveil's reminder "look at the top two cards of your library, then put any number of them into your graveyard and the rest on top of your library in any order". The comma sits here, not in the nested tail: a nested glyph opening with a comma doesn't match.</summary>
/// <exampledoc>Aven Fateshaper</exampledoc>
/// <examplecapture>look at the top four cards of your library, then put them back in any order</examplecapture>
public class LookAndRearrange : Glyph
{
    public override Nib[] Nibs => ["look at", Prop(What), ", then put", Prop(Rearrangement)];

    public LookedAtThing What { get; set; }
    public OneOf<PutThemBack, SplitLookedAtCards> Rearrangement { get; set; }
}
