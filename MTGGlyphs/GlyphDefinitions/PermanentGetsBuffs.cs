namespace MTGGlyphs.GlyphDefinitions;

/// <summary>
/// A permanent gaining or losing abilities or power/toughness, for a while or for good: "target creature gains
/// trample and gets +x/+0 until end of turn", "enchanted creature has flying".
/// </summary>
public class PermanentGetsBuffs : Glyph
{
    public override Nib[] Nibs => [Prop(Permanent), Prop(Buffs), Prop(Duration)];

    public BuffedPermanent Permanent { get; set; }
    public OneOf<ManyOf<GainedOrLostBuff>, GainedOrLostBuff> Buffs { get; set; }

    [Optional]
    public PhaseTiming Duration { get; set; }
}

[Dependent]
public class BuffedPermanent : GlyphOneOf
{
    public Target Target { get; set; }
    public EnchantedPermanent EnchantedPermanent { get; set; }
    public This This { get; set; }
}
