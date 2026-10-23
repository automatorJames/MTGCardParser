namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Mnemonic Betrayal, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mnemonic Betrayal</exampledoc>
[Dependent]
public class MnemonicBetrayalEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if any of those cards remain exiled, return them to their owners' graveyards"];
}
