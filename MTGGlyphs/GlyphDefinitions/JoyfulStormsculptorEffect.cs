namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Joyful Stormsculptor, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Joyful Stormsculptor</exampledoc>
[Dependent]
public class JoyfulStormsculptorEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature deals 1 damage to each opponent and each battle they protect"];
}
