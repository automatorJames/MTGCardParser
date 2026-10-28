namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A status or designation a permanent can have or become: "face-down", "face-up", "suspected", "modified", "goaded", "saddled", "monstrous", "renowned", "equipped", "enchanted", "transformed", "prepared", "plotted" - as a quality ("face-down creature"), a state it becomes ("becomes renowned") or a condition ("while saddled").</summary>
public enum PermanentDesignation
{
    [RegexPattern("face-down")]
    FaceDown,

    [RegexPattern("face-up")]
    FaceUp,

    Suspected,

    Modified,

    Goaded,

    Saddled,

    Monstrous,

    Renowned,

    Equipped,

    Enchanted,

    Transformed,

    Prepared,

    Bargained,

    Harnessed,

    Plotted
}
