namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Keldon Firebombers, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Keldon Firebombers</exampledoc>
[Dependent]
public class KeldonFirebombersEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each player sacrifices all lands they control except for three"];
}
