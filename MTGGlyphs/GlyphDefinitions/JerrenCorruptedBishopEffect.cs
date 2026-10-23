namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Jerren, Corrupted Bishop // Ormendahl, the Corrupter, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Jerren, Corrupted Bishop // Ormendahl, the Corrupter</exampledoc>
[Dependent]
public class JerrenCorruptedBishopEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you lose 1 life and create a 1/1 white human creature token"];
}
