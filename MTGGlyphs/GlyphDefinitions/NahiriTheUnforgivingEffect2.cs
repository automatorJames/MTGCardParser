namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Nahiri, the Unforgiving, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Nahiri, the Unforgiving</exampledoc>
[Dependent]
public class NahiriTheUnforgivingEffect2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["until your next turn, up to one target creature attacks a player each combat if able"];
}
