namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Blade of Shared Souls, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Blade of Shared Souls</exampledoc>
[TokenizationOrder(-1)]
public class BladeOfSharedSoulsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever this equipment becomes attached to a creature, for as long as this equipment remains attached to it, you may have that creature become a copy of another target creature you control"];
}
