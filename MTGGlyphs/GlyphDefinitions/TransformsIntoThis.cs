namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A back face's trigger on the card transforming into it, where the back face's own name has become {this}: "whenever this creature transforms into {this}, …". The front face's "enters or transforms into" names the back face as text instead (see <see cref="EntersOrTransformsInto"/>).</summary>
/// <exampledoc>Afflicted Deserter // Werewolf Ransacker</exampledoc>
[Dependent]
public class TransformsIntoThis : Glyph
{
    public override Nib[] Nibs => ["transforms into", Nib.This];
}
