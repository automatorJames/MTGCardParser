namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Furnace Punisher, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Furnace Punisher</exampledoc>
[Dependent]
public class FurnacePunisherEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature deals 2 damage to that player unless they control two or more basic lands"];
}
