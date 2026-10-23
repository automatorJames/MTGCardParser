namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Favored Hoplite, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Favored Hoplite</exampledoc>
/// <examplecapture>put a +1/+1 counter on this creature and prevent all damage that would be dealt to it this turn</examplecapture>
[Dependent]
public class FavoredHopliteEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put a +1/+1 counter on this creature and prevent all damage that would be dealt to it this turn"];
}
