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

        //result = result.Where(x => x.Name == "Ankh of Mishra");
        //result = result.Where(x => x.Name == "Berserk");
        ////result = [new Card { Name = "feckall", Text = "as long as enchanted land is a land, it's a permanent artifact" }];
        //result = [new Card { Name = "Animate Dead", Text = "when Animate Dead enters the battlefield, if it's on the battlefield, it loses \"enchant creature card in a graveyard\" and gains \"enchant creature put onto the battlefield with Animate Dead.\" Regardless, afterward go fuck yourself with a spoon." }];
        //result = result.Where(x => x.Name == "Animate Dead");
        return result.ToList();
    }
}

