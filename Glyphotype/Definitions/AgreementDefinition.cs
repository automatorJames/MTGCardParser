namespace Glyphotype.Definitions;

/// <summary>
/// A referent's agreement features, as an <see cref="IntroducesAttribute"/> declares them for what it introduces, or
/// an <see cref="AgreementAttribute"/> for what a <see cref="BackReference"/> requires.
/// </summary>
public sealed record AgreementDefinition
{
    public GrammaticalNumber Number { get; init; }

    /// <summary>The kind of thing (e.g. "creature"), or null when unspecified.</summary>
    public string Kind { get; init; }

    public static AgreementDefinition Of(IntroducesAttribute introduces) =>
        introduces is null ? null : new() { Number = introduces.Number, Kind = introduces.Kind };

    public static AgreementDefinition Of(AgreementAttribute agreement) =>
        agreement is null ? null : new() { Number = agreement.Number, Kind = agreement.Kind };
}
