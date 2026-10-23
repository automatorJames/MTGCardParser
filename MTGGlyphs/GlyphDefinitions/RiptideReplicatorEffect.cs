namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Riptide Replicator, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Riptide Replicator</exampledoc>
[Dependent]
public class RiptideReplicatorEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create an x/x creature token of the chosen color and type, where x is the number of charge counters on this artifact"];
}
