namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Lolth, Spider Queen, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Lolth, Spider Queen</exampledoc>
/// <examplecapture>you get an emblem with "whenever an opponent is dealt combat damage by one or more creatures you control, if that player lost less than 8 life this turn, they lose life equal to the difference</examplecapture>
[Dependent]
public class LolthSpiderQueenEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you get an emblem with \"whenever an opponent is dealt combat damage by one or more creatures you control, if that player lost less than 8 life this turn, they lose life equal to the difference"];
}
