namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Carpet of Flowers, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Carpet of Flowers</exampledoc>
/// <examplecapture>if you haven't added mana with this ability this turn, you may add x mana of any one color, where x is the number of islands target opponent controls</examplecapture>
[Dependent]
public class CarpetOfFlowersEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you haven't added mana with this ability this turn, you may add x mana of any one color, where x is the number of islands target opponent controls"];
}
