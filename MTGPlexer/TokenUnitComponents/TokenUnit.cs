namespace MTGPlexer.TokenUnitComponents;

public abstract class TokenUnit
{
    public RegexTemplate Template { get; init; }

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
    public string CapturePath { get; set; }
    public List<TokenUnit> ChildTokenUnits { get; set; } = [];

    /// <summary>
    /// A pre-processed and ordered list of all property captures for this token.
    /// This is the preferred way to iterate over captures for rendering or processing.
    /// </summary>
    public List<IndexedPropertyCapture> IndexedPropertyCaptures { get; set; } = [];

    protected TokenUnit(params string[] templateSnippets)
    {
        if (templateSnippets.Length == 0 && !TokenTypeRegistry.Templates.ContainsKey(Type))
        {
            // If children pass no arguments or call the default parameterless base constructor,
            // we assume they want to construct snippets from their ordered properties.

            templateSnippets = Type
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                .Select(x => x.Name)
                .ToArray();
        }

        // Always check the static registry first, since constructing the template is somewhat heavy
        // (not much, but it adds up over all instantiations across large bodies of text)
        if (TokenTypeRegistry.Templates.ContainsKey(Type))
            Template = TokenTypeRegistry.Templates[Type];
        else
            Template = new(Type, templateSnippets);

        OnInitialized();
    }

    protected virtual void OnInitialized()
    {
        // Base implementation requires no initialization
    }

    protected virtual void OnAfterHydrated()
    {
        // Base implementation requires no actions post-hydration
    }

    public static TokenUnit HydrateFromMatch(Type tokenUnitType, Match match, string capturePath = null, string distinguishingAppendix = null, int captureIndex = 0)
    {
        var tokenUnit = (TokenUnit)Activator.CreateInstance(tokenUnitType);
        tokenUnit.TopLevelMatch = match;
        tokenUnit.Capture = match;
        tokenUnit.CapturePath = capturePath ?? tokenUnitType.Name;

        foreach (var captureProp in tokenUnit.Template.CaptureGroupProps)
        {
            if (match.Groups[captureProp.Name + distinguishingAppendix].Success && captureIndex <= match.Groups[captureProp.Name + distinguishingAppendix].Captures.Count - 1)
                captureProp.SetValueFromMatch(tokenUnit, match, captureIndex, distinguishingAppendix);
            else if (tokenUnitType.IsAssignableTo(typeof(TokenUnitOneOf)))
                Debug.WriteLine($"TokenUnit.HydrateFromMatch: TokenUnitOneOf Match '{match.Value}' contains no named capture group '{captureProp.Name + distinguishingAppendix}'");
            else
                throw new Exception($"No capture group named '{captureProp.Name}' at capture index {captureIndex} exists for match '{match.Value}'");
        }

        tokenUnit.OnAfterHydrated();

        return tokenUnit;
    }

    public TokenUnit HydrateAsChildFromCapture(
        Type tokenUnitType, 
        Match match, 
        Capture childCapture, 
        string ancestorCapturePath, 
        string distinguishingAppendix = null,
        bool addToTokenChildUnits = true,
        int captureIndex = 0
        )
    {
        var tokenUnitChild = HydrateFromMatch(tokenUnitType, match, ancestorCapturePath, distinguishingAppendix, captureIndex);

        // overwrite the child's Capture property
        tokenUnitChild.Capture = childCapture;

        if (addToTokenChildUnits)
            ChildTokenUnits.Add(tokenUnitChild);

        return tokenUnitChild;
    }

    public void SetPropertyFromCapture(RegexPropInfo regexPropInfo, Capture capture, object propVal, string distinguishingAppendix = null)
    {
        regexPropInfo.Prop.SetValue(this, propVal);
        var capturePosition = IndexedPropertyCaptures.Count;
        IndexedPropertyCaptures.Add(new(regexPropInfo, capture, propVal, capturePosition, CapturePath, distinguishingAppendix));
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
        if (string.IsNullOrEmpty(Template.RegexString))
            return $"{nameof(Template.RegexString)} is null or empty";

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