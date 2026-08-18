using Dapper;
using Microsoft.Data.SqlClient;

namespace MTGGlyphs.Data;

public class CardDataGetter : IDocumentRepository
{
    readonly string _sqlConnString;
    int? _maxSetSequence;
    bool _includeEmptyDocuments;

    public CardDataGetter(GlobalSettings settings)
    {
        _sqlConnString = settings.SqlConnString;
        _maxSetSequence = settings.MaxSetSequence;
        _includeEmptyDocuments = settings.IncludeEmptyDocuments;
    }

    public async Task<List<IDocument>> GetDocumentsAsync()
    {
        var conditions = new List<string>();

        if (_maxSetSequence.HasValue)
            conditions.Add("SetSequence <= @MaxSequence");

        if (!_includeEmptyDocuments)
            conditions.Add("Text is not null");

        var whereClause = conditions.Count > 0
            ? " where " + string.Join(" and ", conditions)
            : string.Empty;

        var query = "select * from Card" + whereClause;
        using var conn = new SqlConnection(_sqlConnString);
        var result = await conn.QueryAsync<Card>(query, new { MaxSequence = _maxSetSequence });

        result = result.Where(x => x.Name == "Ankh of Mishra");

        ////result = [new Card { Name = "feckall", Text = "as long as enchanted land is a land, it's a permanent artifact" }];
        //result = result.Where(x => x.Name == "Animate Artifact");
        //result = [new Card { Name = "feckall", Text = "as long as enchanted artifact isn't a creature, it's an artifact creature with power and toughness each equal to its mana value." }];
        //result = result.Where(x => x.Name == "Aspect of Wolf");
        //result = result.Where(x => x.Name == "Ancestral Recall");
        //result = result.Where(x => x.Name == "Berserk");
        //result = [new Card { Name = "feckall", Text = "Target creature gains trample and gets +X/+0 until end of turn, where X is its power." }];
        //result = [new Card { Name = "feck you all", Text = "draw three cards" }, new Card { Name = "feck them all", Text = "draws seven cards" }];
        //result = [new Card { Name = "A", Text = "target a, a, and a" }, new Card { Name = "B", Text = "target b, b, or b" }];
        //result = [new Card { Name = "Ancestrall Recall", Text = "target player draws three cards." }];

        //result = [
        //    new Card { Name = "Ancestrall Recall", Text = "target player draws three cards." }, 
        //    new Card { Name = "Ancestrall Recall Deux", Text = "target opponent draws three cards." },
        //    new Card { Name = "Ancestrall Recall Sevaughn", Text = "target opponent draws seven cards." },
        //    new Card { Name = "Silly Salve", Text = "target player you gain three life." }];

        return result.Cast<IDocument>().ToList();
    }
}

