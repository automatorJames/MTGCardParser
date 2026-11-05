namespace MTGEngine.Core;

/// <summary>
/// Level up and class levels. A leveler's "level 1-3" and "level 4+" lines head the stats and abilities it has while
/// its level counters are in that range; a class's "{r}{g}: level 2" heads the abilities it has from that level on.
/// Each such ability becomes one that holds only then: stats and keywords as statics with that condition, statics
/// and triggers with it added, activated abilities limited to it.
/// </summary>
public static class Levels
{
    static Condition Counters(string kind, int count, Comparison comparison) =>
        new HasCountersCondition(new Self(), new CounterSpec(kind, new Bound(new Fixed(count), comparison)));

    static Condition Range(LevelRange range) =>
        range.OrMore || range.High <= 0
            ? Counters("level", range.Low, Comparison.GreaterOrEqual)
            : new And([Counters("level", range.Low, Comparison.GreaterOrEqual), Counters("level", range.High, Comparison.LessOrEqual)]);

    /// <summary>A class is level 1 to begin with; each level gained after that is a "class-level" counter.</summary>
    static Condition ClassLevel(int level) => Counters("class-level", level - 1, Comparison.GreaterOrEqual);

    static Condition Both(Condition first, Condition second) => first is null ? second : new And([first, second]);

    public static IEnumerable<Ability> Rewrite(IEnumerable<Ability> abilities)
    {
        Condition holds = null;
        var list = abilities.ToList();
        for (var i = 0; i < list.Count; i++)
        {
            var ability = list[i];
            switch (ability)
            {
                case LevelRange range:
                    holds = Range(range);
                    continue;
                case ActivatedAbility { Effect: GainClassLevel gain } activated:
                    holds = ClassLevel(gain.Level);
                    // Gaining a level is a sorcery-speed ability, and only for the next level.
                    yield return activated;
                    yield return new ActivationLimits([new Timing(nameof(TimingKind.SorcerySpeed)), new Timing("If", If: Counters("class-level", gain.Level - 2, Comparison.Equal))]);
                    continue;
            }
            if (holds is null)
            {
                yield return ability;
                continue;
            }
            switch (ability)
            {
                case StatsLine stats:
                    yield return new StaticAbility(new Pump(new Self(), BaseStats: new StatsMod(stats.Power, stats.Toughness)), holds);
                    break;
                case KeywordLine line:
                    yield return new StaticAbility(new GrantAbility(new Self(), line), holds);
                    break;
                case StaticAbility statics:
                    yield return statics with { While = Both(statics.While, holds) };
                    break;
                case TriggeredAbility triggered:
                    yield return triggered with { If = Both(triggered.If, holds) };
                    break;
                case ActivatedAbility activated:
                    yield return activated;
                    if (i + 1 < list.Count && list[i + 1] is ActivationLimits limits)
                    {
                        yield return limits with { Timings = [.. limits.Timings, new Timing("If", If: holds)] };
                        i++;
                    }
                    else
                        yield return new ActivationLimits([new Timing("If", If: holds)]);
                    break;
                default:
                    yield return ability;
                    break;
            }
        }
    }
}
