namespace Glyphotype.Tests;

/// <summary>Reading some of a corpus's volumes: what a repository serves for each selection, with volumes or without.</summary>
public sealed class VolumeSelectionTests
{
    sealed record Document(string Name, string Text) : IDocument;

    /// <summary>Three volumes, numbered 1, 2 and 4, of one document each - serving ranges by way of its listed volumes.</summary>
    sealed class VolumedRepository : IDocumentRepository
    {
        readonly Dictionary<int, IDocument> _byVolume = new()
        {
            [1] = new Document("one", "one"),
            [2] = new Document("two", "two"),
            [4] = new Document("four", "four"),
        };

        public Task<List<IDocument>> GetDocumentsAsync() => Task.FromResult(_byVolume.Values.ToList());

        public Task<List<IDocument>> GetDocumentsAsync(params int[] volumes) =>
            Task.FromResult(_byVolume.Where(x => volumes.Contains(x.Key)).Select(x => x.Value).ToList());

        public int[] GetVolumes() => [.. _byVolume.Keys];
    }

    sealed class UnvolumedRepository : IDocumentRepository
    {
        public Task<List<IDocument>> GetDocumentsAsync() => Task.FromResult<List<IDocument>>([new Document("all", "all")]);
    }

    static async Task<string[]> Read(VolumeSelection selection, IDocumentRepository repository) =>
        (await selection.GetDocumentsAsync(repository)).Select(x => x.Name).ToArray();

    [Fact]
    public async Task Each_selection_reads_its_volumes()
    {
        var repository = new VolumedRepository();

        Assert.Equal(["one", "two", "four"], await Read(VolumeSelection.All, repository));
        Assert.Equal(["two", "four"], await Read(VolumeSelection.Range(2, 4), repository));
        Assert.Equal(["one", "four"], await Read(VolumeSelection.Only(4, 1), repository));
    }

    [Fact]
    public async Task A_corpus_without_volumes_is_read_whole_whatever_the_selection()
    {
        var repository = new UnvolumedRepository();

        Assert.Empty(((IDocumentRepository)repository).GetVolumes());
        Assert.Equal(["all"], await Read(VolumeSelection.Range(2, 4), repository));
        await Assert.ThrowsAsync<NotSupportedException>(() => ((IDocumentRepository)repository).GetDocumentsAsync(1, 2));
    }

    [Fact]
    public void Selections_compare_and_describe_by_what_they_pick()
    {
        Assert.Equal(VolumeSelection.Only(3, 1), VolumeSelection.Only(1, 3));
        Assert.Equal(VolumeSelection.Range(5, 1), VolumeSelection.Range(1, 5));
        Assert.NotEqual(VolumeSelection.Range(1, 5), VolumeSelection.All);

        Assert.Equal("all volumes", VolumeSelection.All.Describe());
        Assert.Equal("volumes 1 to 5", VolumeSelection.Range(1, 5).Describe());
        Assert.Equal("volume 3", VolumeSelection.Only(3).Describe());
        Assert.Equal("volumes 1, 3 and 7", VolumeSelection.Only(7, 1, 3).Describe());
    }
}
