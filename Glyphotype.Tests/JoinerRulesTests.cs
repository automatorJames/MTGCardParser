using Glyphotype.RegexGeneration.Graph;

namespace Glyphotype.Tests;

/// <summary>
/// <see cref="JoinerRules"/> as a table: each row is one situation between two neighbours and what the rules
/// decide for it. The corpus tests exercise the same rules end to end; these pin down each rule on its own.
/// </summary>
public class JoinerRulesTests
{
    const string NoText = null;

    [Theory]
    // group joiner       one-of  before-prop after-prop after-text  plural clause  separated   => expected
    [InlineData(Joiner.Space,      false, true,  true,  NoText,     false, false, false, Joiner.Space)]      // two properties: the group's joiner
    [InlineData(Joiner.CommaSpace, false, true,  true,  NoText,     false, false, false, Joiner.CommaSpace)]
    [InlineData(Joiner.None,       false, true,  true,  NoText,     false, false, false, Joiner.None)]       // a fused group joins with nothing
    [InlineData(Joiner.Space,      false, true,  false, "with",     false, false, false, Joiner.Space)]      // ordinary text after a property
    [InlineData(Joiner.Space,      false, true,  true,  NoText,     false, false, true,  Joiner.None)]       // the regex so far already ends in a space
    [InlineData(Joiner.Space,      false, true,  false, ",",        false, false, false, Joiner.None)]       // tight punctuation binds to the token before
    [InlineData(Joiner.Space,      false, true,  false, "'s",       false, false, false, Joiner.None)]
    [InlineData(Joiner.Space,      false, true,  false, @"\.",      false, true,  false, Joiner.None)]       // a clause break hugs the clause it ends
    [InlineData(Joiner.Space,      false, true,  false, "(s|es|ies)?", true, false, false, Joiner.None)]     // a plural suffix is part of the word before
    [InlineData(Joiner.Space,      false, true,  false, "[ ]and",   false, false, false, Joiner.None)]       // text supplying its own leading space
    [InlineData(Joiner.Pipe,       true,  true,  true,  NoText,     false, false, false, Joiner.Pipe)]       // one-of: the pipe between two alternatives
    [InlineData(Joiner.Pipe,       true,  false, true,  NoText,     false, false, false, Joiner.None)]       // one-of: nothing between its text and an alternative
    [InlineData(Joiner.Pipe,       true,  true,  false, @"\}",      false, false, false, Joiner.None)]
    public void Between_decides_what_separates_two_neighbours(
        Joiner groupJoiner, bool groupIsOneOf, bool beforeIsProperty, bool afterIsProperty,
        string afterText, bool afterIsPluralSuffix, bool afterIsClauseBreak, bool alreadySeparated, Joiner expected)
    {
        var site = new JoinSite(groupJoiner, groupIsOneOf, beforeIsProperty, afterIsProperty, afterText, afterIsPluralSuffix, afterIsClauseBreak, alreadySeparated);

        Assert.Equal(expected, JoinerRules.Between(site));
    }

    [Theory]
    // first  nullable anchor-before => placement
    [InlineData(true,  false, false, JoinerPlacement.None)]              // a first child has nothing to be separated from
    [InlineData(true,  true,  false, JoinerPlacement.None)]
    [InlineData(false, true,  false, JoinerPlacement.InsideNodeLeading)] // a nullable node carries its own joiner...
    [InlineData(false, true,  true,  JoinerPlacement.InsideNodeLeading)] // ...whatever precedes it
    [InlineData(false, false, true,  JoinerPlacement.BeforeNode)]        // a required node after a guaranteed one: unconditionally
    [InlineData(false, false, false, JoinerPlacement.None)]              // a required node after only nullables: its predecessor carries it
    public void PlaceLeadingJoiner_decides_where_the_joiner_goes(bool isFirstChild, bool isNullable, bool hasAnchorBefore, JoinerPlacement expected)
    {
        Assert.Equal(expected, JoinerRules.PlaceLeadingJoiner(isFirstChild, isNullable, hasAnchorBefore));
    }

    [Theory]
    // nullable has-next next-nullable next-anchor-before => owns
    [InlineData(true,  true,  false, false, true)]   // the last nullable before an unanchored required node carries its joiner
    [InlineData(true,  true,  false, true,  false)]  // anchored: the next node places its own
    [InlineData(true,  true,  true,  false, false)]  // the next node is nullable too: it carries its own
    [InlineData(true,  false, false, false, false)]  // nothing follows
    [InlineData(false, true,  false, false, false)]  // only a nullable node can carry it
    public void OwnsTrailingJoiner_hands_an_unanchored_joiner_to_the_nullable_before_it(bool isNullable, bool hasNext, bool nextIsNullable, bool nextHasAnchorBefore, bool expected)
    {
        Assert.Equal(expected, JoinerRules.OwnsTrailingJoiner(isNullable, hasNext, nextIsNullable, nextHasAnchorBefore));
    }

    [Theory]
    // compound-of property-joined-by type-joined-by => repetition joiner
    [InlineData(false, Joiner.Space,      Joiner.Space, Joiner.CommaSpace)] // ManyOf: "a, b, and c", regardless
    [InlineData(true,  null,              null,         Joiner.CommaSpace)] // CompoundOf's default
    [InlineData(true,  null,              Joiner.Space, Joiner.Space)]      // a subclass's [JoinedBy]
    [InlineData(true,  Joiner.CommaSpace, Joiner.Space, Joiner.CommaSpace)] // the property's [JoinedBy] wins
    [InlineData(true,  Joiner.None,       null,         Joiner.None)]       // fused
    public void ForRepetition_decides_the_separator_between_repeated_items(bool ownerIsCompoundOf, Joiner? propertyJoinedBy, Joiner? typeJoinedBy, Joiner expected)
    {
        Assert.Equal(expected, JoinerRules.ForRepetition(ownerIsCompoundOf, propertyJoinedBy, typeJoinedBy));
    }

    [Theory]
    [InlineData(",", true)]
    [InlineData("as though it didn't", false)]
    [InlineData("it's", false)]
    [InlineData("until end of turn.", true)]
    [InlineData("", false)]
    public void AdheresToPrecedingText_keeps_a_joiner_with_text_ending_in_tight_punctuation(string precedingText, bool expected)
    {
        Assert.Equal(expected, JoinerRules.AdheresToPrecedingText(precedingText));
    }
}
