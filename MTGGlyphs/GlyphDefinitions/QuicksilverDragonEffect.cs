namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Quicksilver Dragon, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Quicksilver Dragon</exampledoc>
[Dependent]
public class QuicksilverDragonEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if target spell has only one target and that target is this creature, change that spell's target to another creature"];
}
