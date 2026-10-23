namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Blade of Shared Souls, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Blade of Shared Souls</exampledoc>
/// <examplecapture>for as long as this equipment remains attached to it, you may have that creature become a copy of another target creature you control</examplecapture>
[Dependent]
public class BladeOfSharedSoulsEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["for as long as this equipment remains attached to it, you may have that creature become a copy of another target creature you control"];
}
