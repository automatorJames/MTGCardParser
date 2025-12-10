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

    public Match TopLevelMatch { get; set; }
    public Capture Capture { get; set; }
    public CaptureGroupPropPath CapturePath { get; set; }
    public List<TokenUnit> ChildTokenUnits { get; set; } = [];

    /// <summary>
    /// A pre-processed and ordered list of all property captures for this token.
    /// This is the preferred way to iterate over captures for rendering or processing.
    /// </summary>
    public List<IndexedPropertyCapture> IndexedPropertyCaptures { get; set; } = [];

    public void HydrateFromMatch(TypeMatch typeMatch)
    {
        TopLevelMatch = typeMatch.Match;
        Capture = typeMatch.ChildCapture ?? typeMatch.Match;
        CapturePath = typeMatch.CapturePath != null ? typeMatch.CapturePath : new(Type.Name);

        if (!TokenTypeRegistry.Templates.TryGetValue(Type, out var template))
            return;

        foreach (var captureProp in template.CaptureGroupProps)
        {
            //if (typeMatch.Match.Groups[captureProp.Name + typeMatch.DistinguishingAppendix].Success && typeMatch.CaptureIndex <= typeMatch.Match.Groups[captureProp.Name + typeMatch.DistinguishingAppendix].Captures.Count - 1)
            if (typeMatch[captureProp.Name + typeMatch.DistinguishingAppendix] != null && typeMatch.CaptureIndex <= typeMatch[captureProp.Name + typeMatch.DistinguishingAppendix].Captures.Count - 1)
                captureProp.SetValueFromMatch(this, typeMatch.Match, typeMatch.CaptureIndex, typeMatch.DistinguishingAppendix);
            else if (Type.IsAssignableTo(typeof(TokenUnitOneOf)))
                Debug.WriteLine($"TokenUnit.HydrateFromMatch: TokenUnitOneOf Match '{typeMatch.Match.Value}' contains no named capture group '{captureProp.Name + typeMatch.DistinguishingAppendix}'");
            else
                throw new Exception($"No capture group named '{captureProp.Name}' at capture index {typeMatch.CaptureIndex} exists for match '{typeMatch.Match.Value}'");
        }

        OnAfterHydrated();
    }

    public string[] GetSnippets() => Snippets;

    protected virtual void OnAfterHydrated()
    {
        // Base implementation requires no actions post-hydration
    }

    public static TokenUnit InstantiateFromMatch(Type tokenUnitType, TypeMatch typeMatch)
    {
        var instance = Activator.CreateInstance(tokenUnitType);

        if (instance is not TokenUnit tokenUnitInstance)
            throw new Exception($"Type '{tokenUnitType}' isn't a {nameof(TokenUnit)} type");

        tokenUnitInstance.HydrateFromMatch(typeMatch);

        return tokenUnitInstance;
    }

    public void SetPropertyFromCapture(RegexPropInfo regexPropInfo, Capture capture, object propVal, string distinguishingAppendix = null)
    {
        regexPropInfo.Prop.SetValue(this, propVal);
        var capturePosition = IndexedPropertyCaptures.Count;
        IndexedPropertyCapture indexedPropertyCapture = new(regexPropInfo, capture, propVal, capturePosition, CapturePath, distinguishingAppendix);
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
    public override string ToString() => $"{Type.Name}{(Capture == null ? "" : $": {Capture.Value}")}";
}