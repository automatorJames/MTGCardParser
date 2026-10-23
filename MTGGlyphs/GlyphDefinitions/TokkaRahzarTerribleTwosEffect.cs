namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Tokka & Rahzar, Terrible Twos, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tokka &amp; Rahzar, Terrible Twos</exampledoc>
/// <examplecapture>if the amount of mana spent to cast it was less than its mana value, tokka &amp; rahzar deal 3 damage to that player</examplecapture>
[Dependent]
public class TokkaRahzarTerribleTwosEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if the amount of mana spent to cast it was less than its mana value, tokka & rahzar deal 3 damage to that player"];
}
