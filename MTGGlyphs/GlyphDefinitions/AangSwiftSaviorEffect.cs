namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Aang, Swift Savior // Aang and La, Ocean's Fury, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Aang, Swift Savior // Aang and La, Ocean's Fury</exampledoc>
[Dependent]
public class AangSwiftSaviorEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["airbend up to one other target creature or spell"];
}
