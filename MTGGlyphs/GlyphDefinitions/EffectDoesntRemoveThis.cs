namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The Ward cycle's exception to protection: "this effect doesn't remove {this}" - the aura granting protection isn't removed by it.</summary>
/// <exampledoc>White Ward</exampledoc>
/// <examplecapture>this effect doesn't remove {this}</examplecapture>
public class EffectDoesntRemoveThis : Glyph
{
    public override Nib[] Nibs => ["this effect doesn't remove", Nib.This];
}
