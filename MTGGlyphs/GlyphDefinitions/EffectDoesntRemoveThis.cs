namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The Ward cycle's exception to protection: "this effect doesn't remove this aura" - the aura granting protection isn't removed by it.</summary>
/// <exampledoc>White Ward</exampledoc>
/// <examplecapture>this effect doesn't remove this aura</examplecapture>
public class EffectDoesntRemoveThis : Glyph
{
    public override Nib[] Nibs => ["this effect doesn't remove", Prop(Aura)];

    public OneOf<This, ThisCreature> Aura { get; set; }
}
