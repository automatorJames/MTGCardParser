namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Ensouled Scimitar, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ensouled Scimitar</exampledoc>
[Dependent]
public class EnsouledScimitarEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["equipment that's a creature can't equip a creature"];
}
