namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Curse of the Werefox, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Curse of the Werefox</exampledoc>
/// <examplecapture>creatures that fight each deal damage equal to their power to the other</examplecapture>
[TokenizationOrder(-1)]
public class CurseOfTheWerefoxRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["creatures that fight each deal damage equal to their power to the other"];
}
