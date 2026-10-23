namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Anzrag, the Quake-Mole, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Anzrag, the Quake-Mole</exampledoc>
[Dependent]
public class AnzragTheQuakeMoleEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["anzrag must be blocked each combat this turn if able"];
}
