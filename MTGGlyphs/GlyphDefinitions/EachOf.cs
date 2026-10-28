namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Each member of a group, affected one by one: several targets ("each of two target creatures", "each of up to two target creatures", "each of up to two targets") or a group named earlier in the line ("each of them", "each of those creatures", "each of those permanents", "each of those vampires").</summary>
/// <exampledoc>Swelter</exampledoc>
/// <examplecapture>each of two target creatures</examplecapture>
[Dependent]
public class EachOf : Glyph
{
    public override Nib[] Nibs => ["each of", Prop(Group)];

    public OneOf<CountedTargets, CountedAnyTargets, Them, ThatCard, ThatCardPhrase, ThatCreatureOfType> Group { get; set; }
}
