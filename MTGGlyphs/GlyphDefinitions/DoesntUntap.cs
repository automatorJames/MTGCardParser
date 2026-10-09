namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A permanent that stays tapped through its controller's untap step: "doesn't untap during your untap step", "don't untap during their controllers' untap steps".</summary>
/// <exampledoc>Paralyze</exampledoc>
/// <examplecapture>doesn't untap during its controller's untap step</examplecapture>
[Dependent]
public class DoesntUntap : Glyph, IPredicate
{
    public override Nib[] Nibs => [Alt("doesn't", "don't"), "untap during", Prop(Step)];

    public PlayersPhase Step { get; set; }
}
