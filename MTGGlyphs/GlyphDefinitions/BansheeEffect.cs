namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Banshee, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Banshee</exampledoc>
[Dependent]
public class BansheeEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature deals half x damage, rounded down, to any target, and half x damage, rounded up, to you"];
}
