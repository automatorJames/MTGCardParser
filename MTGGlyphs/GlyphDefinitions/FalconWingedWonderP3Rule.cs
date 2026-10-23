namespace MTGGlyphs.GlyphDefinitions;

[TokenizationOrder(-1)]
public class FalconWingedWonderP3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["avian telepathy — when falcon enters, create redwing, a legendary 1/1 blue bird scout creature token with flying and \"whenever redwing attacks, surveil 1"];
}
