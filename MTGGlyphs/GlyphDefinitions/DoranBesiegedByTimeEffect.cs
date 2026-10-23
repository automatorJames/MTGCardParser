namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Doran, Besieged by Time, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Doran, Besieged by Time</exampledoc>
/// <examplecapture>it gets +x/+x until end of turn, where x is the difference between its power and toughness</examplecapture>
[Dependent]
public class DoranBesiegedByTimeEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it gets +x/+x until end of turn, where x is the difference between its power and toughness"];
}
