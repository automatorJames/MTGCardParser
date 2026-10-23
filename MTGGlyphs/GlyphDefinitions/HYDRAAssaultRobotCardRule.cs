namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of HYDRA Assault Robot, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>HYDRA Assault Robot</exampledoc>
[TokenizationOrder(-1)]
public class HYDRAAssaultRobotCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever another villain and/or artifact you control enters, this creature deals 1 damage to target opponent"];
}
