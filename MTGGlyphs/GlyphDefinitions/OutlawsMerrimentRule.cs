namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Outlaws' Merriment, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Outlaws' Merriment</exampledoc>
[TokenizationOrder(-1)]
public class OutlawsMerrimentRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["• 1/2 human rogue with haste and \"when this token enters, it deals 1 damage to any target"];
}
