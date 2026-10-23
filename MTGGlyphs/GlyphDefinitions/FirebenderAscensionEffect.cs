namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Firebender Ascension, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Firebender Ascension</exampledoc>
[Dependent]
public class FirebenderAscensionEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put a quest counter on this enchantment"];
}
