namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Zombie Assassin, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Zombie Assassin</exampledoc>
[Dependent]
public class ZombieAssassinEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy target nonblack creature"];
}
