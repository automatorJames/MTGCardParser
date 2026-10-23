namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Faunsbane Troll, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Faunsbane Troll</exampledoc>
[Dependent]
public class FaunsbaneTrollEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature fights target creature you don't control"];
}
