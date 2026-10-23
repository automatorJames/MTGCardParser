namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Tovolar, Dire Overlord // Tovolar, the Midnight Scourge, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tovolar, Dire Overlord // Tovolar, the Midnight Scourge</exampledoc>
[TokenizationOrder(-1)]
public class TovolarDireOverlordRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then transform any number of human werewolves you control"];
}
