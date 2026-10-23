namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Mishra's War Machine, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mishra's War Machine</exampledoc>
[TokenizationOrder(-1)]
public class MishrasWarMachineRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it deals damage to you this way, tap it"];
}
