namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Musician, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Musician</exampledoc>
/// <examplecapture>if it doesn't have "at the beginning of your upkeep, destroy this creature unless you pay {1} for each music counter on it," it gains that ability</examplecapture>
[TokenizationOrder(-1)]
public class MusicianRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it doesn't have \"at the beginning of your upkeep, destroy this creature unless you pay {1} for each music counter on it,\" it gains that ability"];
}
