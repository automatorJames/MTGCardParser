namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Antagonism, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Antagonism</exampledoc>
/// <examplecapture>this enchantment deals 2 damage to that player unless one of their opponents was dealt damage this turn</examplecapture>
[Dependent]
public class AntagonismEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this enchantment deals 2 damage to that player unless one of their opponents was dealt damage this turn"];
}
