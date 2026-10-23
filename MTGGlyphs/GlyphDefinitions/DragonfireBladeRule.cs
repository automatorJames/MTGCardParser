namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Dragonfire Blade, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Dragonfire Blade</exampledoc>
[TokenizationOrder(-1)]
public class DragonfireBladeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this ability costs {1} less to activate for each color of the creature it targets"];
}
