namespace Glyphotype.Interfaces;

/// <summary>
/// Where a corpus's documents come from. A corpus may divide its documents into numbered volumes (see
/// <see cref="GetVolumes"/>), so that part of it can be read without the rest; one that doesn't implements only
/// <see cref="GetDocumentsAsync()"/>.
/// </summary>
public interface IDocumentRepository
{
    /// <summary>Every document in the corpus, from every volume.</summary>
    public Task<List<IDocument>> GetDocumentsAsync();

    /// <summary>The documents in the volumes numbered <paramref name="minVolume"/> to <paramref name="maxVolume"/>, inclusive.</summary>
    /// <exception cref="NotSupportedException">The corpus isn't divided into volumes.</exception>
    public Task<List<IDocument>> GetDocumentsAsync(int minVolume, int maxVolume) =>
        GetDocumentsAsync(GetVolumes().Where(x => x >= minVolume && x <= maxVolume).ToArray());

    /// <summary>The documents in the volumes numbered <paramref name="volumes"/> - none, if none are given.</summary>
    /// <exception cref="NotSupportedException">The corpus isn't divided into volumes.</exception>
    public Task<List<IDocument>> GetDocumentsAsync(params int[] volumes) =>
        throw new NotSupportedException($"{GetType().Name} doesn't divide its documents into volumes");

    /// <summary>The numbers of the corpus's volumes, in order - empty for a corpus that isn't divided into volumes.</summary>
    public int[] GetVolumes() => [];
}
