namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"target spell or permanent": a target that may be on the stack or on the battlefield.</summary>
/// <exampledoc>Chaoslace</exampledoc>
/// <examplecapture>target spell or permanent</examplecapture>
[Dependent]
public class TargetSpellOrPermanent : Glyph
{
    public override Nib[] Nibs => ["target spell or permanent"];
}
