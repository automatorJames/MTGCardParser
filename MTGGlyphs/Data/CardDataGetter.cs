using Dapper;
using Microsoft.Data.SqlClient;

namespace MTGGlyphs.Data;

/// <summary>The card database's cards, as a corpus whose volumes are its sets, numbered by set sequence.</summary>
public class CardDataGetter : IDocumentRepository
{
    readonly string _sqlConnString;
    readonly bool _includeEmptyDocuments;
    readonly Lazy<int[]> _volumes;
    readonly Lazy<IReadOnlyDictionary<int, string>> _volumeNames;

    public CardDataGetter(GlobalSettings settings)
    {
        _sqlConnString = settings.SqlConnString;
        _includeEmptyDocuments = settings.IncludeEmptyDocuments;
        _volumes = new(ReadVolumes, LazyThreadSafetyMode.PublicationOnly);
        _volumeNames = new(ReadVolumeNames, LazyThreadSafetyMode.PublicationOnly);
    }

    public Task<List<IDocument>> GetDocumentsAsync() =>
        QueryAsync(null, null);

    public Task<List<IDocument>> GetDocumentsAsync(int minVolume, int maxVolume) =>
        QueryAsync("SetSequence between @MinVolume and @MaxVolume", new { MinVolume = minVolume, MaxVolume = maxVolume });

    public Task<List<IDocument>> GetDocumentsAsync(params int[] volumes) =>
        QueryAsync("SetSequence in @Volumes", new { Volumes = volumes });

    /// <summary>The set sequences of the sets with cards in them - read once, since sets don't change while the app runs.</summary>
    public int[] GetVolumes() => _volumes.Value;

    /// <summary>The names of the sets with cards in them, by set sequence - read once, as <see cref="GetVolumes"/> is.</summary>
    public IReadOnlyDictionary<int, string> GetVolumeNames() => _volumeNames.Value;

    IReadOnlyDictionary<int, string> ReadVolumeNames()
    {
        using var conn = new SqlConnection(_sqlConnString);

        return conn.Query<(int Volume, string Name)>("select distinct c.SetSequence, s.name from Card c join [Set] s on s.code = c.SetCode" + Where(null))
            .ToDictionary(x => x.Volume, x => x.Name);
    }

    async Task<List<IDocument>> QueryAsync(string volumeCondition, object parameters)
    {
        using var conn = new SqlConnection(_sqlConnString);
        var result = await conn.QueryAsync<Card>("select * from Card" + Where(volumeCondition), parameters);

        return result.Cast<IDocument>().ToList();
    }

    int[] ReadVolumes()
    {
        using var conn = new SqlConnection(_sqlConnString);

        return conn.Query<int>("select distinct SetSequence from Card" + Where(null) + " order by SetSequence").ToArray();
    }

    string Where(string volumeCondition)
    {
        var conditions = new List<string>();

        if (volumeCondition is not null)
            conditions.Add(volumeCondition);

        if (!_includeEmptyDocuments)
            conditions.Add("Text is not null");

        return conditions.Count > 0
            ? " where " + string.Join(" and ", conditions)
            : string.Empty;
    }
}
