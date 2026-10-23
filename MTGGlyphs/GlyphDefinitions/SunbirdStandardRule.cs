namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Sunbird Standard // Sunbird Effigy, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sunbird Standard // Sunbird Effigy</exampledoc>
/// <examplecapture>craft with one or more {5} ({5}, exile this artifact, exile one or more other permanents you control and/or cards from your graveyard: return this card transformed under its owner's control</examplecapture>
[TokenizationOrder(-1)]
public class SunbirdStandardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["craft with one or more {5} ({5}, exile this artifact, exile one or more other permanents you control and/or cards from your graveyard: return this card transformed under its owner's control"];
}
