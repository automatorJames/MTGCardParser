namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Phyrexian Splicer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Phyrexian Splicer</exampledoc>
/// <examplecapture>until end of turn, target creature with the chosen ability loses it and another target creature gains it</examplecapture>
[Dependent]
public class PhyrexianSplicerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["until end of turn, target creature with the chosen ability loses it and another target creature gains it"];
}
