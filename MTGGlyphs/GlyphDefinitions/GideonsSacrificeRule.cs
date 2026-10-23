namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Gideon's Sacrifice, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Gideon's Sacrifice</exampledoc>
/// <examplecapture>all damage that would be dealt this turn to you and permanents you control is dealt to the chosen permanent instead (if it's still on the battlefield)</examplecapture>
[TokenizationOrder(-1)]
public class GideonsSacrificeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["all damage that would be dealt this turn to you and permanents you control is dealt to the chosen permanent instead (if it's still on the battlefield)"];
}
