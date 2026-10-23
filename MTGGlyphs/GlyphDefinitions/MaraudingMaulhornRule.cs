namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Marauding Maulhorn, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Marauding Maulhorn</exampledoc>
[TokenizationOrder(-1)]
public class MaraudingMaulhornRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature attacks each combat if able unless you control a creature named advocate of the beast"];
}
