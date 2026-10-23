namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Startled Awake // Persistent Nightmare, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Startled Awake // Persistent Nightmare</exampledoc>
[Dependent]
public class StartledAwakeEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put this card from your graveyard onto the battlefield transformed"];
}
