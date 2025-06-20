namespace MTGCardParser;

internal class Program
{
    static void Main(string[] args)
    {
        var tokenTester = new TokenTester(10, 1, true);
        tokenTester.Process();
    }
}
