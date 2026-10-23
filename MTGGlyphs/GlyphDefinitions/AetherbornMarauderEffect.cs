namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Aetherborn Marauder, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Aetherborn Marauder</exampledoc>
[Dependent]
public class AetherbornMarauderEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["move any number of +1/+1 counters from other permanents you control onto this creature"];
}
