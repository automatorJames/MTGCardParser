namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"until {this} leaves the battlefield", "until this creature leaves the battlefield": an exile that lasts only as long as this permanent stays.</summary>
/// <exampledoc>Banisher Priest</exampledoc>
/// <examplecapture>until this creature leaves the battlefield</examplecapture>
[Dependent]
public class UntilThisLeaves : Glyph
{
    public override Nib[] Nibs => ["until", Prop(Source), "leaves the battlefield"];

    public OneOf<This, ThisCreature> Source { get; set; }
}
