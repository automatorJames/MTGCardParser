namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Curse of Misfortunes, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Curse of Misfortunes</exampledoc>
/// <examplecapture>you may search your library for a curse card that doesn't have the same name as a curse attached to enchanted player, put it onto the battlefield attached to that player, then shuffle</examplecapture>
[Dependent]
public class CurseOfMisfortunesEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may search your library for a curse card that doesn't have the same name as a curse attached to enchanted player, put it onto the battlefield attached to that player, then shuffle"];
}
