namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Anafenza, the Foremost, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Anafenza, the Foremost</exampledoc>
/// <examplecapture>if a nontoken creature an opponent owns would die or a creature card not on the battlefield would be put into an opponent's graveyard, exile that card instead</examplecapture>
[TokenizationOrder(-1)]
public class AnafenzaTheForemostRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if a nontoken creature an opponent owns would die or a creature card not on the battlefield would be put into an opponent's graveyard, exile that card instead"];
}
