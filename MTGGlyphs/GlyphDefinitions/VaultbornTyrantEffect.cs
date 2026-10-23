namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Vaultborn Tyrant, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Vaultborn Tyrant</exampledoc>
/// <examplecapture>if it's not a token, create a token that's a copy of it, except it's an artifact in addition to its other types</examplecapture>
[Dependent]
public class VaultbornTyrantEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it's not a token, create a token that's a copy of it, except it's an artifact in addition to its other types"];
}
