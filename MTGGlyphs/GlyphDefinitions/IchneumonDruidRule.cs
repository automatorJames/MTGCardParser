namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Ichneumon Druid, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ichneumon Druid</exampledoc>
[TokenizationOrder(-1)]
public class IchneumonDruidRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever an opponent casts an instant spell other than the first instant spell that player casts each turn, this creature deals 4 damage to that player"];
}
