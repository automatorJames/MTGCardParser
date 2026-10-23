namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Drizzt Do'Urden, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Drizzt Do'Urden</exampledoc>
[Dependent]
public class DrizztDoUrdenEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it had power greater than drizzt's power, put a number of +1/+1 counters on drizzt equal to the difference"];
}
