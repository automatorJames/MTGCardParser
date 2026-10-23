namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of The Skullspore Nexus, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Skullspore Nexus</exampledoc>
/// <examplecapture>create a green fungus dinosaur creature token with base power and toughness each equal to the total power of those creatures</examplecapture>
[Dependent]
public class TheSkullsporeNexusEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create a green fungus dinosaur creature token with base power and toughness each equal to the total power of those creatures"];
}
