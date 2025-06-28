namespace MTGCardParser;

internal class Program
{
    static void Main(string[] args)
    {
        TypeRegistry.Initialize();
        //var cards = DataGetter.GetCards(1, true);
        //var result = MtgTextAnalyzer.GetCoveragePatterns(cards);

        var tokenTester = new TokenTester(1, true);
        tokenTester.Process(hydrateAllTokenInstances: true);
    }
}
