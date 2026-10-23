namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Battle Cry Goblin, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Battle Cry Goblin</exampledoc>
/// <examplecapture>if you attacked with creatures with total power 6 or greater this combat, create a 1/1 red goblin creature token that's tapped and attacking</examplecapture>
[Dependent]
public class BattleCryGoblinEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you attacked with creatures with total power 6 or greater this combat, create a 1/1 red goblin creature token that's tapped and attacking"];
}
