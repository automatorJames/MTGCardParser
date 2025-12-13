namespace MTGPlexer.TokenUnitComponents;

public abstract class TokenUnit
{
    protected virtual string[] Snippets { get; } = [];

    Type _type;
    public Type Type
    {
        get
        {
            if (_type is null)
                _type = GetType();

            return _type;
        }
    }

    //public Capture Capture => Match.RegexMatch;
    public TokenUnitMatch Match { get; set; }
    public List<TokenUnit> ChildTokenUnits { get; set; } = [];

    /// <summary>
    /// A pre-processed and ordered list of all property captures for this token.
    /// This is the preferred way to iterate over captures for rendering or processing.
    /// </summary>
    public List<IndexedPropertyCapture> IndexedPropertyCaptures { get; set; } = [];

    public string[] GetSnippets() => Snippets;

    protected virtual void OnAfterHydrated()
    {
        // Base implementation requires no actions post-hydration
    }

    public static TokenUnit InstantiateFromMatch(TokenUnitMatch match)
    {
        if (!match.Type.IsAssignableTo(typeof(TokenUnit)))
            throw new Exception($"Type '{match.Type.Name}' isn't a {nameof(TokenUnit)} type");

        var tokenUnitInstance = (TokenUnit)Activator.CreateInstance(match.Type);
        tokenUnitInstance.Match = match;

        if (!TokenTypeRegistry.Templates.TryGetValue(match.Type, out var template))
            return tokenUnitInstance;

        foreach (var captureProp in template.CaptureGroupProps)
        {
            if (match[captureProp.Name] != null && match.CaptureIndex <= match[captureProp.Name].Captures.Count - 1)
                captureProp.SetValueFromNamedGroupInMatch(tokenUnitInstance);
            else if (match.Type.IsAssignableTo(typeof(TokenUnitOneOf)))
                Debug.WriteLine($"TokenUnit.HydrateFromMatch: TokenUnitOneOf Match '{match.RegexMatch.Value}' contains no named capture group '{captureProp.Name}'");
            else
                throw new Exception($"No capture group named '{captureProp.Name}' at capture index {match.CaptureIndex} exists for match '{match.RegexMatch.Value}'");
        }

        tokenUnitInstance.OnAfterHydrated();

        return tokenUnitInstance;
    }

    public void SetPropertyFromCapture(RegexPropInfo regexPropInfo, Capture capture, object propVal)
    {
        regexPropInfo.Prop.SetValue(this, propVal);
        var capturePosition = IndexedPropertyCaptures.Count;
        IndexedPropertyCapture indexedPropertyCapture = new(regexPropInfo, capture, propVal, capturePosition, Match.CapturePath);
        IndexedPropertyCaptures.Add(indexedPropertyCapture);
    }

    /// <summary>
    /// Returns a list of this TokenUnit's IndexedPropertyCaptures where RegexPropInfo.IsTerminal, and
    /// recursively gathers terminal captures from all TokenUnit children.
    /// </summary>
    public List<IndexedPropertyCapture> GetFlattenedTerminalCaptures()
    {
        var terminalCaptures = IndexedPropertyCaptures
            .Where(x => x.RegexPropInfo.IsTerminal)
            .ToList();

        ChildTokenUnits.ForEach(x => terminalCaptures.AddRange(x.GetFlattenedTerminalCaptures()));
        terminalCaptures.AddRange(FlattenManyOfCaptures());

        return terminalCaptures;
    }

    /// <summary>
    /// Recursively processes all ManyOf props into terminals (ManyItemVariant.Enum). Also returns 
    /// the ManyOf.Conjunction value, if any, for both ManyItemVariant.Enum and ManyItemVariant.TokenUnit.
    /// </summary>
    public List<IndexedPropertyCapture> FlattenManyOfCaptures()
    {
        List<IndexedPropertyCapture> terminalCaptures = [];

        var manyOfPropCaps = IndexedPropertyCaptures
            .Where(x => x.Value is ManyOf manyOf)
            .ToList();

        foreach (var manyOfPropCap in manyOfPropCaps)
        {
            var manyOf = (ManyOf)manyOfPropCap.Value;

            foreach (var manyItemOrdinal in Enum.GetValues<ManyItemOrdinal>())
            {
                // We only get the first (if any) among the items at the current ordinal, because its IndexedPropertyCaptures
                // will contain all captures at that position (i.e. _secondPlus) , and we don't want to duplicate those captures
                var representativeManyItemAtOrdinal = manyOf.ItemObjects.FirstOrDefault(x => x.Oridinal == manyItemOrdinal);

                if (representativeManyItemAtOrdinal == null)
                    continue;

                if (manyOf.ManyItemVariant == ManyItemVariant.Enum)
                {
                    var derivedPropCapture = manyOfPropCap.DeriveForManyOfItem(manyOf, representativeManyItemAtOrdinal);
                    terminalCaptures.Add(derivedPropCapture);
                }
                else if (manyOf.ManyItemVariant == ManyItemVariant.TokenUnit)
                {
                    var derivedPropCapture = manyOfPropCap.DeriveForManyOfItem(manyOf, representativeManyItemAtOrdinal);
                    var manyOfItemTokenUnit = (TokenUnit)representativeManyItemAtOrdinal.ItemObject;
                    terminalCaptures.AddRange(manyOfItemTokenUnit.GetFlattenedTerminalCaptures());
                }
            }

            // For both enums (terminals) and branches (TokenUnits), both of which may have a terminal Conjunction
            if (manyOf.Conjunction != null)
            {
                var derivedPropCapture = manyOfPropCap.DeriveForManyOfConjunction(manyOf);
                terminalCaptures.Add(derivedPropCapture);
            }
        }

        return terminalCaptures;
    }


    /// <summary>
    /// Only intended to be called by TokenTypeRegistry once upon startup. May be overridden by
    /// inheriting abstract classes who want to specify their own validation requirements.
    /// </summary>
    public virtual string ValidateStructure()
    {
        var template = TokenTypeRegistry.Templates[Type];

        if (string.IsNullOrEmpty(template.RegexString))
            return $"{nameof(template.RegexString)} is null or empty";

        return null;
    }

    /// <summary>
    /// Called after hydration to ensure the token conforms to expected data requirements.
    /// May be overridden by inheriting abstract classes who want to specify their own validation 
    /// requirements, and may be overriden by concrete classes for type-specific requirements.
    /// </summary>
    public virtual bool ValidateHydratedToken()
    {
        return true;
    }

    //public override string ToString() => $"{Type.Name}{(MatchSpan.Source is null ? "" : $": {MatchSpan.ToStringValue()}")}";
    //public override string ToString() => $"{Type.Name}{(Match == null ? "" : $": {Ma.Value}")}";
}