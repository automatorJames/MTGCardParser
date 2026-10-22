namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A trigger condition joining casting a spell and a permanent entering: "you cast a black spell or a swamp you control enters" (the Magus staffs).</summary>
/// <exampledoc>Staff of the Death Magus</exampledoc>
/// <examplecapture>you cast a black spell or a swamp you control enters</examplecapture>
[Dependent]
public class CastOrEntersEvent : Glyph
{
    public override Nib[] Nibs => [Prop(Caster), "cast", Prop(Spell), "or", Prop(Permanent), "enters"];

    public WhichPlayer Caster { get; set; }
    public IndefiniteSpell Spell { get; set; }
    public IndefinitePermanent Permanent { get; set; }
}
