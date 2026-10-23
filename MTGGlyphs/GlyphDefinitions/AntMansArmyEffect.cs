namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Ant-Man's Army, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ant-Man's Army</exampledoc>
[Dependent]
public class AntMansArmyEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create a food token or a treasure token"];
}
