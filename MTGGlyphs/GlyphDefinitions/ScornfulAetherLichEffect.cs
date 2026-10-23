namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Scornful Aether-Lich, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Scornful Aether-Lich</exampledoc>
[Dependent]
public class ScornfulAetherLichEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["attacking doesn't cause it to tap, and it can't be blocked except by artifact creatures and/or black creatures"];
}
