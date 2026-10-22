namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A second kind of card a search may find instead: "and/or desert cards", "or gate card", "or town card".</summary>
/// <exampledoc>Map the Frontier</exampledoc>
/// <examplecapture>and/or desert cards</examplecapture>
[Dependent]
public class AlternativeWantedKind : Glyph
{
    public override Nib[] Nibs => [Alt("and/or", "or"), Prop(Kind), Plural("card")];

    public PermanentKind Kind { get; set; }
}
