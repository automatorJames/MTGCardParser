namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Indestructible's reminder text.</summary>
/// <exampledoc>Darksteel Colossus</exampledoc>
/// <examplecapture>damage and effects that say "destroy" don't destroy it</examplecapture>
public class IndestructibleRule : Glyph
{
    public override Nib[] Nibs => ["damage and effects that say \"destroy\" don't destroy it"];
}
