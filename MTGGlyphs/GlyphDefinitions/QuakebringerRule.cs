namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Quakebringer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Quakebringer</exampledoc>
[TokenizationOrder(-1)]
public class QuakebringerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this ability triggers only if", Nib.This, "is on the battlefield or if", Nib.This, "is in your graveyard and you control a giant"];
}
