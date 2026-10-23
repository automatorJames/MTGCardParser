namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Liberator, Urza's Battlethopter, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Liberator, Urza's Battlethopter</exampledoc>
/// <examplecapture>if the amount of mana spent to cast that spell is greater than liberator's power, put a +1/+1 counter on liberator</examplecapture>
[Dependent]
public class LiberatorUrzasBattlethopterEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if the amount of mana spent to cast that spell is greater than liberator's power, put a +1/+1 counter on liberator"];
}
