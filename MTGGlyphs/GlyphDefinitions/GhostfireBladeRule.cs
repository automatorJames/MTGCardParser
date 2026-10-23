namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Ghostfire Blade, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ghostfire Blade</exampledoc>
[TokenizationOrder(-1)]
public class GhostfireBladeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this equipment's equip ability costs {2} less to activate if it targets a colorless creature"];
}
