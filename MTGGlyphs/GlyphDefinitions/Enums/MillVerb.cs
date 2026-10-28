namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The verb "mill" ("mills"), kept apart from draw and discard so "mill two cards" stays a <see cref="Mill"/> while "mills cards equal to its power" is measured like a draw.</summary>
[OptionalPlural]
public enum MillVerb
{
    Mill
}
