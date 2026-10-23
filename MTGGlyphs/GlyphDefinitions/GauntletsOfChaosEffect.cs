namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Gauntlets of Chaos, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Gauntlets of Chaos</exampledoc>
/// <examplecapture>exchange control of target artifact, creature, or land you control and target permanent an opponent controls that shares one of those types with it</examplecapture>
[Dependent]
public class GauntletsOfChaosEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exchange control of target artifact, creature, or land you control and target permanent an opponent controls that shares one of those types with it"];
}
