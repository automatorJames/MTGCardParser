namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Diseased Vermin, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Diseased Vermin</exampledoc>
/// <examplecapture>this creature deals x damage to target opponent previously dealt damage by it, where x is the number of infection counters on it</examplecapture>
[Dependent]
public class DiseasedVerminEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature deals x damage to target opponent previously dealt damage by it, where x is the number of infection counters on it"];
}
