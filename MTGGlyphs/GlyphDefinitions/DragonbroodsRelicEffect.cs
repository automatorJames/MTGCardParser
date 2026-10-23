namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Dragonbroods' Relic, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Dragonbroods' Relic</exampledoc>
[Dependent]
public class DragonbroodsRelicEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create a 4/4 dragon creature token named reliquary dragon that's all colors"];
}
