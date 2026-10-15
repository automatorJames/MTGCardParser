namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Banding's damage rule, from its reminder text (banding or "bands with other"): the banding side's controller divides the combat damage of a creature blocking or blocked by the band. Whether it applies while blocking as well as attacking, and who divides, vary.</summary>
/// <exampledoc>Benalish Hero</exampledoc>
/// <examplecapture>if any creatures with banding you control are blocking or being blocked by a creature, you divide that creature's combat damage, not its controller, among any of the creatures it's being blocked by or is blocking</examplecapture>
public class BandingDamageRule : Glyph
{
    public override Nib[] Nibs => ["if", Prop(Band), "are", Prop(WhileBlocking), "being blocked by", Alt("a", "the same"), "creature,", Prop(Divider), Pattern("divides?"), "that creature's combat damage, not its controller, among any of the creatures", Alt("it's being blocked by or is blocking", "it's blocking")];

    public OneOf<BandingCreatures, BandsWithOtherCreatures> Band { get; set; }
    [RegexPattern("blocking or")]
    public bool WhileBlocking { get; set; }
    public OneOf<ThatPlayer, WhichPlayer?> Divider { get; set; }
}
