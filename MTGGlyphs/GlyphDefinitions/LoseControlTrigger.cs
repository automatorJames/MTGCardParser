namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A trigger on losing control of a permanent: "when you lose control of this enchantment, …", "when you lose control of the creature, tap it".</summary>
/// <exampledoc>Duplicity</exampledoc>
/// <examplecapture>when you lose control of this enchantment, put all cards exiled with this enchantment into their owner's graveyard</examplecapture>
public class LoseControlTrigger : Glyph
{
    public override Nib[] Nibs => ["when you lose control of", Prop(Permanent), ",", Prop(Effect)];

    public PermanentPhrase Permanent { get; set; }
    public DynamicGlyph Effect { get; set; }
}
