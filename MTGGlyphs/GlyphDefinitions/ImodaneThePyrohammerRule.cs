namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Imodane, the Pyrohammer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Imodane, the Pyrohammer</exampledoc>
[TokenizationOrder(-1)]
public class ImodaneThePyrohammerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever an instant or sorcery spell you control that targets only a single creature deals damage to that creature, imodane deals that much damage to each opponent"];
}
