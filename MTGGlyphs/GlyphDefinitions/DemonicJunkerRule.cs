namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Demonic Junker, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Demonic Junker</exampledoc>
[TokenizationOrder(-1)]
public class DemonicJunkerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if a creature you controlled was destroyed this way, put two +1/+1 counters on this vehicle"];
}
