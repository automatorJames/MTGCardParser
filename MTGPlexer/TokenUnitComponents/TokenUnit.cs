namespace MTGPlexer.TokenUnitComponents;

public abstract class TokenUnit
{
    protected virtual Snippet[] Snippets { get; } = [];

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

    public TokenUnitMatch Match { get; set; }

    /// <summary>
    /// A pre-processed  list of all property captures for this token.
    /// This is the preferred way to iterate over captures for rendering or processing.
    /// </summary>
    public List<PropertyCapture> PropertyCaptures { get; set; } = [];

    public Snippet[] GetSnippets() => Snippets;

    protected virtual void OnAfterHydrated()
    {
        // Base implementation requires no actions post-hydration
    }

    public static TokenUnit InstantiateFromMatch(TokenUnitMatch match, Dictionary<DynamicOfSegment, object> prefilledDynamicValues = null)
    {
        if (!match.Type.IsAssignableTo(typeof(TokenUnit)))
            throw new Exception($"Type '{match.Type.Name}' isn't a {nameof(TokenUnit)} type");

        prefilledDynamicValues ??= [];

        var tokenUnitInstance = (TokenUnit)Activator.CreateInstance(match.Type);
        tokenUnitInstance.Match = match;

        if (!TokenTypeRegistry.Templates.TryGetValue(match.Type, out var template))
            return tokenUnitInstance;

        foreach (var captureProp in template.CaptureGroupProps)
        {
            if (captureProp is DynamicOfSegment dynamicRegexProp && prefilledDynamicValues.TryGetValue(dynamicRegexProp, out object prefilledValue))
                dynamicRegexProp.SetValueFromPrefilledDynamicToken(tokenUnitInstance, prefilledValue);
            else
            {
                var setSuccessfully = captureProp.TrySetOnParent(tokenUnitInstance);

                // If this is a dynamic prop that failed to match any type regex, the parent TokenUnit is
                // invalid. The reason we check for a valid match this late in the processing pipeline is that
                // we don't want the tokenizer to be responsible for resolving the dynamic property to a match
                // itself, since that would require double work (i.e. first iterate through all regexes to confirm a match,
                // then later iterate through all again to actually assign the match value within in this method).
                if (!setSuccessfully && captureProp.TemplatePropInfo.TemplatePropType == TemplatePropType.Dynamic)
                    return null;
            }
        }

        tokenUnitInstance.OnAfterHydrated();

        return tokenUnitInstance;
    }

    public void SetPropertyFromCapture(TemplatePropInfo templatePropInfo, Capture capture, object propVal)
    {
        templatePropInfo.Prop.SetValue(this, propVal);
        PropertyCapture propertyCapture = new(templatePropInfo, capture, propVal, Match.CapturePath);
        PropertyCaptures.Add(propertyCapture);
    }

    /// <summary>
    /// Returns a list of this TokenUnit's IndexedPropertyCaptures where TemplatePropInfo.IsTerminal, and
    /// recursively gathers terminal captures from all TokenUnit children.
    /// </summary>
    public List<PropertyCapture> GetFlattenedTerminalCaptures()
    {
        var terminalCaptures = PropertyCaptures
            .Where(x => x.TemplatePropInfo.IsTerminal)
            .ToList();

        var childTokenUnits = PropertyCaptures
            .Select(x => x.Value)
            .OfType<TokenUnit>()
            .ToList();

        childTokenUnits.ForEach(x => terminalCaptures.AddRange(x.GetFlattenedTerminalCaptures()));
        terminalCaptures.AddRange(FlattenManyOfCaptures());
        terminalCaptures.AddRange(FlattenCompoundOfCaptures());
        terminalCaptures.AddRange(FlattenOneOfCaptures());

        return terminalCaptures;
    }

    /// <summary>
    /// Recursively processes all ManyOf props into terminals (ManyItemVariant.Enum). Also returns 
    /// the ManyOf.Conjunction value, if any, for both ManyItemVariant.Enum and ManyItemVariant.TokenUnit.
    /// </summary>
    public List<PropertyCapture> FlattenManyOfCaptures()
    {
        List<PropertyCapture> terminalCaptures = [];

        var manyOfPropCaps = PropertyCaptures
            .Where(x => x.Value is ManyOf)
            .ToList();

        foreach (var manyOfPropCap in manyOfPropCaps)
        {
            var manyOf = (ManyOf)manyOfPropCap.Value;

            foreach (var manyItemOrdinal in Enum.GetValues<ManyItemOrdinal>())
            {
                // We only get the first (if any) among the items at the current ordinal, because its IndexedPropertyCaptures
                // will contain all captures at that position (i.e. _secondPlus) , and we don't want to duplicate those captures
                var representativeManyItemAtOrdinal = manyOf.ItemObjects.FirstOrDefault(x => x.DistinguishingName == manyItemOrdinal.ToString());

                if (representativeManyItemAtOrdinal == null)
                    continue;

                if (manyOf.ManyItemVariant == CaptureTypeVariant.Enum)
                {
                    var derivedPropCapture = manyOfPropCap.DeriveForManyOfItem(manyOf, representativeManyItemAtOrdinal);
                    terminalCaptures.Add(derivedPropCapture);
                }
                else if (manyOf.ManyItemVariant == CaptureTypeVariant.TokenUnit)
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

    public List<PropertyCapture> FlattenCompoundOfCaptures()
    {
        List<PropertyCapture> terminalCaptures = [];

        var compoundOfPropCaps = PropertyCaptures
            .Where(x => x.Value is CompoundOf)
            .ToList();

        foreach (var compoundOfPropCap in compoundOfPropCaps)
        {
            var compoundOf = (CompoundOf)compoundOfPropCap.Value;

            foreach (var compoundOfItem in compoundOf.ItemObjects)
            {
                var derivedPropCapture = compoundOfPropCap.DeriveForCompoundOfItem(compoundOf, compoundOfItem);

                if (compoundOf.CaptureTypeVariant == CaptureTypeVariant.Enum)
                    terminalCaptures.Add(derivedPropCapture);
                else if (compoundOf.CaptureTypeVariant == CaptureTypeVariant.TokenUnit)
                {
                    var compoundOfTokenUnit = (TokenUnit)compoundOfItem.ItemObject;
                    terminalCaptures.AddRange(compoundOfTokenUnit.GetFlattenedTerminalCaptures());
                }
            }
        }

        return terminalCaptures;
    }

    public List<PropertyCapture> FlattenOneOfCaptures()
    {
        List<PropertyCapture> terminalCaptures = [];

        var oneOfPropCaps = PropertyCaptures
            .Where(x => x.Value is OneOf)
            .ToList();

        foreach (var oneOfPropCap in oneOfPropCaps)
        {
            var oneOf = (OneOf)oneOfPropCap.Value;

            var derivedPropCapture = oneOfPropCap.DeriveForOneOfItem(oneOf, oneOf.ItemObject);

            if (oneOf.ItemObject.CaptureTypeVariant == CaptureTypeVariant.Enum)
                terminalCaptures.Add(derivedPropCapture);
            else if (oneOf.ItemObject.CaptureTypeVariant == CaptureTypeVariant.TokenUnit)
            {
                var compoundOfTokenUnit = (TokenUnit)oneOf.ItemObject.ItemObject;
                terminalCaptures.AddRange(compoundOfTokenUnit.GetFlattenedTerminalCaptures());
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

        var expectedProps = Type.GetPublicPropNames();
        var missingProps = expectedProps.Except(template.CaptureGroupProps.Select(x => x.TemplatePropInfo.Prop.Name)).ToList();
    
        if (missingProps.Any())
            return $"the following properties are not represented among template snippets: {string.Join(", ", missingProps)}";

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
}