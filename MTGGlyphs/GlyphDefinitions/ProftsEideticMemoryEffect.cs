namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Proft's Eidetic Memory, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Proft's Eidetic Memory</exampledoc>
/// <examplecapture>if you've drawn more than one card this turn, put x +1/+1 counters on target creature you control, where x is the number of cards you've drawn this turn minus one</examplecapture>
[Dependent]
public class ProftsEideticMemoryEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you've drawn more than one card this turn, put x +1/+1 counters on target creature you control, where x is the number of cards you've drawn this turn minus one"];
}
