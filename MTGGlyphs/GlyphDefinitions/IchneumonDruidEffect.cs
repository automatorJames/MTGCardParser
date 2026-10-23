namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Ichneumon Druid, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ichneumon Druid</exampledoc>
/// <examplecapture>this creature deals 4 damage to that player</examplecapture>
[Dependent]
public class IchneumonDruidEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature deals 4 damage to that player"];
}
