namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Cards counted as a group: "cards", as in "you drew two or more cards".</summary>
/// <exampledoc>Archmage Ascension</exampledoc>
/// <examplecapture>cards</examplecapture>
[Dependent]
public class PluralCards : Glyph
{
    public override Nib[] Nibs => ["cards"];
}
