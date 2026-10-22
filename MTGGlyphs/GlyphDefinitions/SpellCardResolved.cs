namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An instant or sorcery card going to the graveyard as it finishes resolving, as a trigger event: "this spell card is put into a graveyard after resolving" (haunt on spells).</summary>
/// <exampledoc>Benediction of Moons</exampledoc>
/// <examplecapture>this spell card is put into a graveyard after resolving</examplecapture>
[Dependent]
public class SpellCardResolved : Glyph
{
    public override Nib[] Nibs => ["this spell card is put into a graveyard after resolving"];
}
