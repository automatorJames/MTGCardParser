namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The ante rule on cards that move cards between players: "remove {this} from your deck before playing if you're not playing for ante".</summary>
/// <exampledoc>Bronze Tablet</exampledoc>
/// <examplecapture>remove {this} from your deck before playing if you're not playing for ante</examplecapture>
public class AnteRemovalRule : Glyph
{
    public override Nib[] Nibs => ["remove", Nib.This, "from your deck before playing if you're not playing for ante"];
}
