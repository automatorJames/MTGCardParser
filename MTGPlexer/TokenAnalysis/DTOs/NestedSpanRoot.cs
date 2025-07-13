namespace MTGPlexer.TokenAnalysis.DTOs;

/// <summary>
/// This is simply a marker record that's identical to its base, NestedSpanBranch.
/// The purpose of this record is sementic: it's the only type that may be passed
/// into an HTML component for rendering.
/// </summary>
public record NestedSpanRoot(TokenUnit Token, string CardName) : NestedSpanBranch(Token, parentPath: CardName, parentDepth: -1)
{
}