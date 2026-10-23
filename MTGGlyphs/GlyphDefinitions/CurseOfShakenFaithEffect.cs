namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Curse of Shaken Faith, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Curse of Shaken Faith</exampledoc>
/// <examplecapture>this aura deals 2 damage to them</examplecapture>
[Dependent]
public class CurseOfShakenFaithEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this aura deals 2 damage to them"];
}
