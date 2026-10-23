namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Spellweaver Helix, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Spellweaver Helix</exampledoc>
/// <examplecapture>if it has the same name as one of the cards exiled with this artifact, you may copy the other</examplecapture>
[Dependent]
public class SpellweaverHelixEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it has the same name as one of the cards exiled with this artifact, you may copy the other"];
}
