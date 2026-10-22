namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Whether, and from where, a player cast this permanent, as a condition: "you cast it from your hand", "you didn't cast it from your hand", "you cast it". The passive "this spell was cast from a graveyard" is <see cref="WasCastFrom"/>.</summary>
/// <exampledoc>Myojin of Cleansing Fire</exampledoc>
/// <examplecapture>you cast it from your hand</examplecapture>
[Dependent]
public class CastByPlayerFrom : Glyph
{
    public override Nib[] Nibs => [Prop(Player), Prop(Negated), "cast it", Prop(From)];

    public WhichPlayer Player { get; set; }
    [Optional]
    [RegexPattern("didn't")]
    public bool Negated { get; set; }
    [Optional]
    public CastFromZone From { get; set; }
}
