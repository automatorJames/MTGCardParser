using System.Collections;

namespace MTGCardParser;

public class TokenCaptureRegistry : IEnumerable<KeyValuePair<MtgToken, string>>
{
    readonly Dictionary<MtgToken, string> _patterns = new();

    public string this[MtgToken token] => _patterns.ContainsKey(token) ? _patterns[token] : null;

    public TokenCaptureRegistry(Dictionary<MtgToken, string> patterns)
    {
        _patterns = patterns;
    }

    public IEnumerator<KeyValuePair<MtgToken, string>> GetEnumerator() => _patterns.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

