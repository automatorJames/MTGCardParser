namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Torgaar, Famine Incarnate, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Torgaar, Famine Incarnate</exampledoc>
[Dependent]
public class TorgaarFamineIncarnateEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["up to one target player's life total becomes half their starting life total, rounded down"];
}
