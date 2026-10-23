namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Gilgamesh, Master-at-Arms, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Gilgamesh, Master-at-Arms</exampledoc>
[TokenizationOrder(-1)]
public class GilgameshMasterAtArmsCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["when you put one or more equipment onto the battlefield this way, you may attach one of them to a samurai you control"];
}
