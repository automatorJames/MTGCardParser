namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Gilgamesh, Master-at-Arms, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Gilgamesh, Master-at-Arms</exampledoc>
[Dependent]
public class GilgameshMasterAtArmsEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may attach one of them to a samurai you control"];
}
