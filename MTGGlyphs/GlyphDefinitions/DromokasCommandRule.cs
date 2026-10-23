namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Dromoka's Command, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Dromoka's Command</exampledoc>
[TokenizationOrder(-1)]
public class DromokasCommandRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["• prevent all damage target instant or sorcery spell would deal this turn"];
}
