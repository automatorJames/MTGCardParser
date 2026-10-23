namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Angel of Destiny, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Angel of Destiny</exampledoc>
/// <examplecapture>if you have at least 15 life more than your starting life total, each player this creature attacked this turn loses the game</examplecapture>
[Dependent]
public class AngelOfDestinyEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you have at least 15 life more than your starting life total, each player this creature attacked this turn loses the game"];
}
