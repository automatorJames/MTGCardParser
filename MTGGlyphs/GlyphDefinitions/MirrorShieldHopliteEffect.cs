namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Mirror-Shield Hoplite, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mirror-Shield Hoplite</exampledoc>
[Dependent]
public class MirrorShieldHopliteEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["copy that ability"];
}
