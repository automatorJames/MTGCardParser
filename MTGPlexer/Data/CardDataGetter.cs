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

        //result = result.Where(x => x.Name == "Animate Dead");
        //result = result.Where(x => x.Name == "Vesuvan Doppelganger");
        //result = result.Where(x => x.Name == "Farmstead");
        //result = new List<Card> { new Card { Name = "baaz", Text = "ABC, DEF, and GHI" } };
        //result = new List<Card> { new Card { Name = "baafsdafgasz", Text = "abc Some fecking intelligible text" } };
        //result = new List<Card> { new Card { Name = "baafsdafgasz", Text = "target player draws three cards or something" } };
        //result = new List<Card> { new Card { Name = "Type1", Text = "Draw three cards" }, new Card { Name = "Type2", Text = "Draws 3 cards" } };
        //result =
        //[ 
        //    new Card { Name = "Type1", Text = "Draw three cards" }, 
        //    new Card { Name = "Type2", Text = "Draws 3 cards" }, 
        //    new Card { Name = "Type3", Text = "Draws fecking shite cards" },
        //    //new Card { Name = "Type3", Text = "Draws holy hell cards" } 
        //];
        //result = new List<Card> { new Card { Name = "Type1", Text = "flying, reach, haste, and trample" }, new Card { Name = "Type2", Text = "flash, fear, or bury" } };
        //result = new List<Card> { new Card { Name = "Type1", Text = "flying, reach, haste, and trample" }, new Card { Name = "Type1Twin", Text = "flying, reach, haste, and trample" }, new Card { Name = "Type2", Text = "flash, fear, or bury" }, new Card { Name = "Other", Text = "at the beginning of your upkeep" } };
        //result = new List<Card> { new Card { Name = "Type1", Text = "flying, reach, haste, and trample" }, new Card { Name = "Type1Twin", Text = "flying, reach, haste, and trample" }, new Card { Name = "Type2", Text = "flash, fear, or bury" } };
        //result = result.Where(x => x.Name == "Berserk"); 
        //result = new List<Card> { new Card { Name = "baaz", Text = "Target creature gains trample and gets +X/+0 until end of turn" } };
        //result = new List<Card> { new Card { Name = "Type1", Text = "creature has flying, reach, haste, and trample" }, new Card { Name = "Type2", Text = "creature has flying, reach, and butt poop" } };
        //result = new List<Card> { new Card { Name = "Type2", Text = "creature has flying, poop fling, butt poop, and haste" } };
        //result = new List<Card> { new Card { Name = "Type2", Text = "target creature has \"poop from a butt\"" } };
        result = result.Where(x => x.Name == "Berserk");
        return result.ToList();
    }
}

