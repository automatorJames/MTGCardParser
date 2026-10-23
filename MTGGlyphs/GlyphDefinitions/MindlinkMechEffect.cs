namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Mindlink Mech, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mindlink Mech</exampledoc>
/// <examplecapture>until end of turn, this vehicle becomes a copy of target nonlegendary creature that crewed it this turn, except it's 4/3, it's a vehicle artifact in addition to its other types, and it has flying</examplecapture>
[Dependent]
public class MindlinkMechEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["until end of turn, this vehicle becomes a copy of target nonlegendary creature that crewed it this turn, except it's 4/3, it's a vehicle artifact in addition to its other types, and it has flying"];
}
