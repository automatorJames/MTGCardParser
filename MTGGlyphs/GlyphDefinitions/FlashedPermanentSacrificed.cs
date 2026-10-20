namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The drawback of flashing in a permanent that's normally sorcery-speed: "if you cast it any time a sorcery couldn't have been cast, the controller of the permanent it becomes sacrifices it at the beginning of the next cleanup step". A fixed rules sentence.</summary>
/// <exampledoc>Armor of Thorns</exampledoc>
/// <examplecapture>if you cast it any time a sorcery couldn't have been cast, the controller of the permanent it becomes sacrifices it at the beginning of the next cleanup step</examplecapture>
public class FlashedPermanentSacrificed : Glyph
{
    public override Nib[] Nibs => ["if you cast it any time a sorcery couldn't have been cast, the controller of the permanent it becomes sacrifices it at the beginning of the next cleanup step"];
}
