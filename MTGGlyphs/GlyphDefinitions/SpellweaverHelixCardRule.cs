namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Spellweaver Helix, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Spellweaver Helix</exampledoc>
[TokenizationOrder(-1)]
public class SpellweaverHelixCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever a player casts a card, if it has the same name as one of the cards exiled with this artifact, you may copy the other"];
}
