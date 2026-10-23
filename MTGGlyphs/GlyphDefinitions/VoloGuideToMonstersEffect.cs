namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Volo, Guide to Monsters, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Volo, Guide to Monsters</exampledoc>
/// <examplecapture>copy that spell</examplecapture>
[Dependent]
public class VoloGuideToMonstersEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["copy that spell"];
}
