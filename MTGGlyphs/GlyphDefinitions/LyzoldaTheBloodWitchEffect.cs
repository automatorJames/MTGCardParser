namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Lyzolda, the Blood Witch, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Lyzolda, the Blood Witch</exampledoc>
[Dependent]
public class LyzoldaTheBloodWitchEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["lyzolda deals 2 damage to any target if the sacrificed creature was red"];
}
