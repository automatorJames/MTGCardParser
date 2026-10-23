namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Werewolf Pack Leader, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Werewolf Pack Leader</exampledoc>
[Dependent]
public class WerewolfPackLeaderEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you attacked with creatures with total power 6 or greater this combat, draw a card"];
}
