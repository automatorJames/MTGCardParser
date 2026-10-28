namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Mindlink Mech, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mindlink Mech</exampledoc>
[TokenizationOrder(-1)]
public class MindlinkMechRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever this vehicle becomes crewed for the first time each turn, until end of turn, this vehicle becomes a copy of target nonlegendary creature that crewed it this turn, except it's 4/3, it's a vehicle artifact in addition to its other types, and it has flying"];
}
