namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Necrotic Plague, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Necrotic Plague</exampledoc>
[Dependent]
public class NecroticPlagueEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["its controller chooses target creature one of their opponents controls"];
}
