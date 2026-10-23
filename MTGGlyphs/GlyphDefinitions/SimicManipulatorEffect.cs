namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Simic Manipulator, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Simic Manipulator</exampledoc>
/// <examplecapture>gain control of target creature with power less than or equal to the number of +1/+1 counters removed this way</examplecapture>
[Dependent]
public class SimicManipulatorEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["gain control of target creature with power less than or equal to the number of +1/+1 counters removed this way"];
}
