namespace MTGRulesIR.Lowering;

/// <summary>
/// The sixteenth round: what a two-faced card's back face says that its front never does. "Transforms into {this}" is
/// the back face's own transform event; "(transforms from …)" is a reminder of the front face and lowers to nothing.
/// </summary>
public sealed partial class Lowerer
{
    void RegisterRoundSixteen()
    {
        Rule<G.TransformsIntoThis>((g, ctx) => new Term(nameof(ObjectEventKind.Transforms), nameof(ObjectEventKind)));
        Rule<G.TransformsFrom>((g, ctx) => null);
    }
}
