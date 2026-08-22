namespace Glyphotype.RegexGeneration.Presentation;

/// <summary>
/// One colored run within a rendered match's content (see <see cref="MatchContentRenderer"/>): either
/// part of the captured match itself - <see cref="FullyQualifiedName"/> set to its most specific named
/// group, for hover correlation against the formatted regex - or up to <see cref="MatchContentRenderer.ContextWordCount"/>
/// words of surrounding context on either side, colored uniformly grey with no <see cref="FullyQualifiedName"/>
/// (so it never participates in hover highlighting as a target, only as a dimmable bystander).
/// </summary>
public record MatchContentSpan(string Content, string FullyQualifiedName, SpanStylePalette Palette);
