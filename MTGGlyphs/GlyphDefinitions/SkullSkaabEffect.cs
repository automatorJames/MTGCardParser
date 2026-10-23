namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Skull Skaab, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Skull Skaab</exampledoc>
[Dependent]
public class SkullSkaabEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create a 2/2 black zombie creature token"];
}
