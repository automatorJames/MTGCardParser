namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The reminder text of the copy-on-cast keywords (storm, replicate, gravestorm): "when you cast this spell, copy it for each spell cast before it this turn", "when you cast this spell, copy it for each time you paid its replicate cost", "when you cast it, copy it for each permanent put into a graveyard this turn".</summary>
/// <exampledoc>Brain Freeze</exampledoc>
/// <examplecapture>when you cast this spell, copy it for each spell cast before it this turn</examplecapture>
public class CopyOnCastRule : Glyph
{
    public override Nib[] Nibs => ["when you cast", Alt("this spell", "it"), ", copy it for each", Prop(Count)];

    public CopyCount Count { get; set; }
}
