namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Banish into Fable, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Banish into Fable</exampledoc>
[Dependent]
public class BanishIntoFableEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["copy it if you control an artifact, then copy it if you control an enchantment"];
}
