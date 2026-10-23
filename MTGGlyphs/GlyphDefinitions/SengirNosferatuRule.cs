namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Sengir Nosferatu, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sengir Nosferatu</exampledoc>
[TokenizationOrder(-1)]
public class SengirNosferatuRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it has \"{1}{b}, sacrifice this token: return an exiled card named", Nib.This, "to the battlefield under its owner's control"];
}
