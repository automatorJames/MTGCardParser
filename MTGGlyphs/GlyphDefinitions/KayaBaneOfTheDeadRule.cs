namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Kaya, Bane of the Dead, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Kaya, Bane of the Dead</exampledoc>
/// <examplecapture>your opponents and permanents your opponents control with hexproof can be the targets of spells and abilities you control as though they didn't have hexproof</examplecapture>
[TokenizationOrder(-1)]
public class KayaBaneOfTheDeadRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["your opponents and permanents your opponents control with hexproof can be the targets of spells and abilities you control as though they didn't have hexproof"];
}
