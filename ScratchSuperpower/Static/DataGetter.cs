using Dapper;
using Microsoft.Data.SqlClient;

namespace MTGCardParser.Static;

public static class DataGetter
{
    const string _connString = "Server=localhost;Database=Magic;Integrated Security=True;MultipleActiveResultSets=True;Command Timeout=3600;TrustServerCertificate=True";

    public static List<Card> GetCards(int? maxSetSequence = null, bool cleanText = true, bool ignoreEmptyText = false)
    {
        var conditions = new List<string>();

        if (maxSetSequence.HasValue)
            conditions.Add("SetSequence <= @MaxSequence");

        if (ignoreEmptyText)
            conditions.Add("Text is not null");

        var whereClause = conditions.Count > 0
            ? " where " + string.Join(" and ", conditions)
            : string.Empty;

        var query = "select * from Card" + whereClause;

        using var conn = new SqlConnection(_connString);
        
        var cards = conn.Query<Card>(query, new { MaxSequence = maxSetSequence }).ToList();
        
        if (cleanText)
            cards.ForEach(x => x.CleanText());

        return cards;
    }

}

