namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Qasali Ambusher, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Qasali Ambusher</exampledoc>
/// <examplecapture>if a creature is attacking you and you control a forest and a plains, you may cast this creature without paying its mana cost and as though it had flash</examplecapture>
[TokenizationOrder(-1)]
public class QasaliAmbusherRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if a creature is attacking you and you control a forest and a plains, you may cast this creature without paying its mana cost and as though it had flash"];
}
