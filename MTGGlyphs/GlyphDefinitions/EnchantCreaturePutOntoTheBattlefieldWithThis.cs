namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"enchant creature put onto the battlefield with {this}": the enchant ability an aura gains once it brings a creature back.</summary>
/// <exampledoc>Animate Dead</exampledoc>
/// <examplecapture>enchant creature put onto the battlefield with {this}</examplecapture>
public class EnchantCreaturePutOntoTheBattlefieldWithThis : Glyph
{
    public override Nib[] Nibs => ["enchant creature put onto the battlefield with", Nib.This];
}