namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Master of Cruelties, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Master of Cruelties</exampledoc>
[Dependent]
public class MasterOfCrueltiesEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["that player's life total becomes 1"];
}
