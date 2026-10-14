using Dapper;
using Microsoft.Data.SqlClient;

namespace MTGGlyphs.Data;

/// <summary>The card database's cards, as a corpus whose volumes are its sets, numbered by set sequence.</summary>
public class CardDataGetter : IDocumentRepository
{
    readonly string _sqlConnString;
    readonly bool _includeEmptyDocuments;
    readonly Lazy<int[]> _volumes;

    public CardDataGetter(GlobalSettings settings)
    {
        _sqlConnString = settings.SqlConnString;
        _includeEmptyDocuments = settings.IncludeEmptyDocuments;
        _volumes = new(ReadVolumes, LazyThreadSafetyMode.PublicationOnly);
    }

    public Task<List<IDocument>> GetDocumentsAsync() =>
        QueryAsync(null, null);

    public Task<List<IDocument>> GetDocumentsAsync(int minVolume, int maxVolume) =>
        QueryAsync("SetSequence between @MinVolume and @MaxVolume", new { MinVolume = minVolume, MaxVolume = maxVolume });

    public Task<List<IDocument>> GetDocumentsAsync(params int[] volumes) =>
        QueryAsync("SetSequence in @Volumes", new { Volumes = volumes });

    /// <summary>The set sequences of the sets with cards in them - read once, since sets don't change while the app runs.</summary>
    public int[] GetVolumes() => _volumes.Value;

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
