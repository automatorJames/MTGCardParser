namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Sagu Pummeler, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sagu Pummeler</exampledoc>
[Dependent]
public class SaguPummelerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put two +1/+1 counters and a reach counter on target creature"];
}
