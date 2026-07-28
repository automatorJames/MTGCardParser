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

    public async Task<List<IDocument>> GetCardsAsync()
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
        ////result = [new Card { Name = "feckall", Text = "as long as enchanted land is a land, it's a permanent artifact" }];
        //result = result.Where(x => x.Name == "Animate Artifact");
        //result = [new Card { Name = "feckall", Text = "as long as enchanted artifact isn't a creature, it's an artifact creature with power and toughness each equal to its mana value." }];
        //result = result.Where(x => x.Name == "Aspect of Wolf");
        //result = result.Where(x => x.Name == "Ancestral Recall");
        //result = result.Where(x => x.Name == "Berserk");
        //result = [new Card { Name = "feckall", Text = "Target creature gains trample and gets +X/+0 until end of turn, where X is its power." }];
        //result = [new Card { Name = "A", Text = "target a, a, and a" }, new Card { Name = "B", Text = "target b, b, or b" }];
        return result.Cast<IDocument>().ToList();
    }
}

