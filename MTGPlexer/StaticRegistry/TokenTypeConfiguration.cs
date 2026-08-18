namespace MTGPlexer.StaticRegistry;

public record TokenTypeConfiguration
(
    Type Type,
    Snippet[] Snippets,
    Joiner ChildJoiner
);