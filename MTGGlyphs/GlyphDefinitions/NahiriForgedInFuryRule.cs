namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Nahiri, Forged in Fury, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Nahiri, Forged in Fury</exampledoc>
[TokenizationOrder(-1)]
public class NahiriForgedInFuryRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may cast equipment spells this way without paying their mana costs"];
}
