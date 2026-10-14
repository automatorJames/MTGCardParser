namespace Glyphotype.Interfaces;

/// <summary>How a <see cref="VolumeSelection"/> picks volumes.</summary>
public enum VolumeSelectionKind
{
    /// <summary>Every volume.</summary>
    All,

    /// <summary>The volumes from <see cref="VolumeSelection.MinVolume"/> to <see cref="VolumeSelection.MaxVolume"/>, inclusive.</summary>
    Range,

    /// <summary>The volumes listed in <see cref="VolumeSelection.Volumes"/>.</summary>
    Only,
}

/// <summary>
/// Which of a corpus's volumes (see <see cref="IDocumentRepository.GetVolumes"/>) to read: every one, a range of them, or
/// a list of them. A corpus that isn't divided into volumes is read whole, whatever the selection.
/// </summary>
public sealed record VolumeSelection
{
    public static VolumeSelection All { get; } = new();

    public VolumeSelectionKind Kind { get; init; }

    public int MinVolume { get; init; }

    public int MaxVolume { get; init; }

    public int[] Volumes { get; init; } = [];

    /// <summary>The volumes from <paramref name="minVolume"/> to <paramref name="maxVolume"/>, inclusive.</summary>
    public static VolumeSelection Range(int minVolume, int maxVolume) =>
        new() { Kind = VolumeSelectionKind.Range, MinVolume = Math.Min(minVolume, maxVolume), MaxVolume = Math.Max(minVolume, maxVolume) };

    /// <summary>The volumes <paramref name="volumes"/>.</summary>
    public static VolumeSelection Only(params int[] volumes) =>
        new() { Kind = VolumeSelectionKind.Only, Volumes = volumes.Distinct().Order().ToArray() };

    /// <summary>The documents in the selected volumes of <paramref name="repository"/> - or all of them, if it isn't divided into volumes.</summary>
    public Task<List<IDocument>> GetDocumentsAsync(IDocumentRepository repository) =>
        Kind == VolumeSelectionKind.All || repository.GetVolumes().Length == 0 ? repository.GetDocumentsAsync()
        : Kind == VolumeSelectionKind.Range ? repository.GetDocumentsAsync(MinVolume, MaxVolume)
        : repository.GetDocumentsAsync(Volumes);

    /// <summary>The selection in a few words: "all volumes", "volumes 1 to 5", "volumes 1, 3 and 7".</summary>
    public string Describe() => Kind switch
    {
        VolumeSelectionKind.Range when MinVolume == MaxVolume => $"volume {MinVolume}",
        VolumeSelectionKind.Range => $"volumes {MinVolume} to {MaxVolume}",
        VolumeSelectionKind.Only when Volumes.Length == 0 => "no volumes",
        VolumeSelectionKind.Only when Volumes.Length == 1 => $"volume {Volumes[0]}",
        VolumeSelectionKind.Only => $"volumes {string.Join(", ", Volumes[..^1])} and {Volumes[^1]}",
        _ => "all volumes",
    };

    public bool Equals(VolumeSelection other) =>
        other is not null && Kind == other.Kind && Kind switch
        {
            VolumeSelectionKind.Range => MinVolume == other.MinVolume && MaxVolume == other.MaxVolume,
            VolumeSelectionKind.Only => Volumes.SequenceEqual(other.Volumes),
            _ => true,
        };

    public override int GetHashCode() => Kind switch
    {
        VolumeSelectionKind.Range => HashCode.Combine(Kind, MinVolume, MaxVolume),
        VolumeSelectionKind.Only => Volumes.Aggregate(Kind.GetHashCode(), (hash, x) => HashCode.Combine(hash, x)),
        _ => Kind.GetHashCode(),
    };
}
