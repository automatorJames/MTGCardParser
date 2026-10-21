namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A trigger on a player tapping a permanent for mana: "whenever you tap a swamp for mana, …", "whenever a player taps a nonbasic land for mana, …".</summary>
/// <exampledoc>Crypt Ghast</exampledoc>
/// <examplecapture>whenever you tap a swamp for mana, add an additional {b}</examplecapture>
public class TapForManaTrigger : Glyph
{
    public override Nib[] Nibs => ["whenever", Prop(Player), Alt("tap", "taps"), Prop(Permanent), "for mana,", Prop(Effect)];

    [Referent]
    public WhichPlayer Player { get; set; }
    [Referent]
    public OneOf<IndefinitePermanent, PermanentPhrase> Permanent { get; set; }
    public DynamicGlyph Effect { get; set; }
}
