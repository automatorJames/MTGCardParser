namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Toralf, God of Fury // Toralf's Hammer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Toralf, God of Fury // Toralf's Hammer</exampledoc>
[TokenizationOrder(-1)]
public class ToralfGodOfFuryCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever a creature or planeswalker an opponent controls is dealt excess noncombat damage, toralf deals damage equal to the excess to any target other than that permanent"];
}
