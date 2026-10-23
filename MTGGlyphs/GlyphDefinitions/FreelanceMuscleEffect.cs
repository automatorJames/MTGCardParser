namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Freelance Muscle, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Freelance Muscle</exampledoc>
/// <examplecapture>it gets +x/+x until end of turn, where x is the greatest power and/or toughness among other creatures you control</examplecapture>
[Dependent]
public class FreelanceMuscleEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it gets +x/+x until end of turn, where x is the greatest power and/or toughness among other creatures you control"];
}
