namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Ellywick Tumblestrum, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ellywick Tumblestrum</exampledoc>
[Dependent]
public class EllywickTumblestrumEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you get an emblem with \"creatures you control have trample and haste and get +2/+2 for each differently named dungeon you've completed"];
}
