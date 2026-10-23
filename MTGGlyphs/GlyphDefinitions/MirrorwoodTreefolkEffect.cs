namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Mirrorwood Treefolk, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mirrorwood Treefolk</exampledoc>
[Dependent]
public class MirrorwoodTreefolkEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the next time damage would be dealt to this creature this turn, that damage is dealt to any target instead"];
}
