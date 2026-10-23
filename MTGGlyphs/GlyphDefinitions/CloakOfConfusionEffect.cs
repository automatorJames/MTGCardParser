namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Cloak of Confusion, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Cloak of Confusion</exampledoc>
[Dependent]
public class CloakOfConfusionEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may have it assign no combat damage this turn"];
}
