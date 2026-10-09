namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The Lace cycle's reminder: a color change leaves mana symbols alone - "mana symbols on that permanent remain unchanged".</summary>
/// <exampledoc>Deathlace</exampledoc>
/// <examplecapture>mana symbols on that permanent remain unchanged</examplecapture>
public class ManaSymbolsUnchanged : Glyph
{
    public override Nib[] Nibs => ["mana symbols on that permanent remain unchanged"];
}
