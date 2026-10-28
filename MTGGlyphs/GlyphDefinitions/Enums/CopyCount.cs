namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What a copy-on-cast keyword's reminder text copies the spell once for each of: "for each spell cast before it this turn" (storm), "for each time you paid its replicate cost" (replicate), "for each permanent put into a graveyard this turn" (gravestorm).</summary>
public enum CopyCount
{
    SpellCastBeforeItThisTurn,
    TimeYouPaidItsReplicateCost,
    PermanentPutIntoAGraveyardThisTurn
}
