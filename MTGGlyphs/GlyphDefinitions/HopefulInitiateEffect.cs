namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Hopeful Initiate, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Hopeful Initiate</exampledoc>
[Dependent]
public class HopefulInitiateEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy target artifact or enchantment"];
}
