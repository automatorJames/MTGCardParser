namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Jerren, Corrupted Bishop // Ormendahl, the Corrupter, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Jerren, Corrupted Bishop // Ormendahl, the Corrupter</exampledoc>
[TokenizationOrder(-1)]
public class JerrenCorruptedBishopCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever jerren enters or another nontoken human you control dies, you lose 1 life and create a 1/1 white human creature token"];
}
