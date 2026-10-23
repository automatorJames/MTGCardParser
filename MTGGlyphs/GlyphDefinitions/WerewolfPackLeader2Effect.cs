namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Werewolf Pack Leader, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Werewolf Pack Leader</exampledoc>
[Dependent]
public class WerewolfPackLeader2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["until end of turn, this creature has base power and toughness 5/3, gains trample, and isn't a human"];
}
