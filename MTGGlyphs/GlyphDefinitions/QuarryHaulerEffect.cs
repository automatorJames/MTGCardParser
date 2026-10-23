namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Quarry Hauler, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Quarry Hauler</exampledoc>
/// <examplecapture>for each kind of counter on target permanent, put another counter of that kind on it or remove one from it</examplecapture>
[Dependent]
public class QuarryHaulerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["for each kind of counter on target permanent, put another counter of that kind on it or remove one from it"];
}
