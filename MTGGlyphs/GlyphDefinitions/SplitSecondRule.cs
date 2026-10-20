namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Split second's reminder text.</summary>
/// <exampledoc>Extirpate</exampledoc>
/// <examplecapture>as long as this spell is on the stack, players can't cast spells or activate abilities that aren't mana abilities</examplecapture>
public class SplitSecondRule : Glyph
{
    public override Nib[] Nibs => ["as long as this spell is on the stack, players can't cast spells or activate abilities that aren't mana abilities"];
}
