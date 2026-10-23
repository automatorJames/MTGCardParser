namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of HYDRA Assault Robot, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>HYDRA Assault Robot</exampledoc>
[Dependent]
public class HYDRAAssaultRobotEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature deals 1 damage to target opponent"];
}
