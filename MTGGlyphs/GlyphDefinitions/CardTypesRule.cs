namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The reminder listing the card types: "artifact, battle, creature, enchantment, instant, kindred, land, planeswalker, and sorcery are card types".</summary>
/// <exampledoc>Blood Oath</exampledoc>
/// <examplecapture>artifact, battle, creature, enchantment, instant, kindred, land, planeswalker, and sorcery are card types</examplecapture>
public class CardTypesRule : Glyph
{
    public override Nib[] Nibs => [Prop(Types), "are card types"];

    public ManyOf<CardType> Types { get; set; }
}
