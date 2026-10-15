namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A forced attack: "{this} attacks each combat if able".</summary>
/// <exampledoc>Juggernaut</exampledoc>
/// <examplecapture>{this} attacks each combat if able</examplecapture>
public class AttacksEachCombat : Glyph
{
    public override Nib[] Nibs => [Nib.This, "attacks each combat if able"];
}
