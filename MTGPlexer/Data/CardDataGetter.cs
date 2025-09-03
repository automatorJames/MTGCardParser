using Dapper;
using Microsoft.Data.SqlClient;

namespace MTGPlexer.Data;

public class CardDataGetter
{
    readonly string _sqlConnString;
    int? _maxSetSequence;
    bool _ignoreEmptyText;

    public CardDataGetter(string sqlConnString, int? maxSetSequence = null, bool ignoreEmptyText = true)
    {
        _sqlConnString = sqlConnString;
        _maxSetSequence = maxSetSequence;
        _ignoreEmptyText = ignoreEmptyText;
    }

    public async Task<List<Card>> GetCardsAsync()
    {
        var conditions = new List<string>();

        if (_maxSetSequence.HasValue)
            conditions.Add("SetSequence <= @MaxSequence");

        if (_ignoreEmptyText)
            conditions.Add("Text is not null");

        var whereClause = conditions.Count > 0
            ? " where " + string.Join(" and ", conditions)
            : string.Empty;

        var query = "select * from Card" + whereClause;
        using var conn = new SqlConnection(_sqlConnString);
        var result = await conn.QueryAsync<Card>(query, new { MaxSequence = _maxSetSequence });

        return result.ToList();
    }
}

