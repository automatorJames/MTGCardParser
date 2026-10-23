namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Volcano Hellion, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Volcano Hellion</exampledoc>
[Dependent]
public class VolcanoHellionEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it deals an amount of damage of your choice to you and target creature"];
}
