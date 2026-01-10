using System.Runtime.CompilerServices;

namespace MTGPlexer.SnippetHelpers;

public static class SnippetShortcuts
{
    public static Snippet Prop(object member, [CallerArgumentExpression("member")] string expression = "")
    {
        // If you pass 'S(CardType)', expression is "CardType".
        // If you pass 'S(this.CardType)', expression is "this.CardType".

        // Optional: If you only want the property name (CardType) 
        // and not the prefix (this.), clean the string:
        var lastDot = expression.LastIndexOf('.');
        var name = lastDot == -1 ? expression : expression[(lastDot + 1)..];

        return new Snippet(name);
    }
}
