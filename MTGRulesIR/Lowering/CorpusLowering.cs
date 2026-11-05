namespace MTGRulesIR.Lowering;

/// <summary>Lowers whole documents: each line's top-level glyphs to abilities, classified by the card's types where the glyph alone doesn't say what frame it is.</summary>
public sealed class CorpusLowering
{
    readonly Lowerer _lowerer;

    public CorpusLowering(Lowerer lowerer = null)
    {
        _lowerer = lowerer ?? new Lowerer();
    }

    public Lowerer Lowerer => _lowerer;

    public CardRules Lower(ProcessedDocument document) =>
        LowerWithOrigins(document).Rules;

    /// <summary>
    /// <paramref name="document"/> lowered, with where in each line's text each node came from, for the nodes whose
    /// source is known: the ones a glyph lowered to, and the line's roots.
    /// </summary>
    public (CardRules Rules, IReadOnlyList<LineOrigins> Origins) LowerWithOrigins(ProcessedDocument document)
    {
        var types = (document.Document as MTGGlyphs.Data.Card)?.Types ?? "";
        List<LineRules> lines = [];
        List<LineOrigins> origins = [];

        foreach (var line in document.Lines)
        {
            var (rules, lineOrigins) = LowerLineWithOrigins(document.Document, types, line);
            lines.Add(rules);
            origins.Add(lineOrigins);
        }

        return (new CardRules(document.Document.Name, types, lines, InfoOf(document.Document as MTGGlyphs.Data.Card)), origins);
    }

    static CardInfo InfoOf(MTGGlyphs.Data.Card card) =>
        card is null ? null : new CardInfo(
            card.ManaCost,
            card.Supertypes,
            card.Subtypes,
            card.Power,
            card.Toughness,
            card.Loyalty,
            string.IsNullOrEmpty(card.Keywords) ? null : card.Keywords.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));

    public LineRules LowerLine(IDocument document, string types, ProcessedLine line) =>
        LowerLineWithOrigins(document, types, line).Rules;

    /// <summary><paramref name="ability"/> with <paramref name="next"/> done after its effect, for the abilities that have one; null for any other.</summary>
    static Ability Continue(Ability ability, Effect next)
    {
        static Effect Then(Effect first, Effect then) =>
            first is Sequence sequence ? new Sequence([.. sequence.Steps, then]) : new Sequence([first, then]);

        return ability switch
        {
            TriggeredAbility triggered => triggered with { Effect = Then(triggered.Effect, next) },
            ActivatedAbility activated => activated with { Effect = Then(activated.Effect, next) },
            LoyaltyAbility loyalty => loyalty with { Effect = Then(loyalty.Effect, next) },
            ChapterAbility chapter => chapter with { Effect = Then(chapter.Effect, next) },
            // "If damage would be dealt to this creature, prevent that damage. Remove a +1/+1 counter from it."
            ReplacementAbility { Effect: Replacement replacement } => new ReplacementAbility(replacement with { Instead = Then(replacement.Instead, next) }),
            LabeledAbility labeled when Continue(labeled.Ability, next) is { } inner => labeled with { Ability = inner },
            FlavorAbility flavor when Continue(flavor.Ability, next) is { } inner => flavor with { Ability = inner },
            _ => null,
        };
    }

    (LineRules Rules, LineOrigins Origins) LowerLineWithOrigins(IDocument document, string types, ProcessedLine line)
    {
        var ctx = new LoweringContext(document, line);
        List<Ability> abilities = [];
        List<bool> reminders = [];

        foreach (var unit in line.Glyphs)
            switch (unit)
            {
                case ClauseBreak:
                    continue;

                case UnmatchedString unmatched:
                    var unlowered = new OpaqueAbility(unmatched.CaptureValue, "Unmatched");
                    ctx.Record(unlowered, unmatched);
                    abilities.Add(unlowered);
                    reminders.Add(false);
                    continue;

                case Glyph glyph:
                    if (_lowerer.Lower(glyph, ctx) is { } node)
                    {
                        var ability = _lowerer.AsAbility(node, ctx, glyph);
                        var reminder = ctx.InParentheses(glyph);
                        // "Whenever …, you may discard a card. If you do, draw a card." The sentences after an ability's
                        // first continue its effect, on the same side of any reminder parenthesis.
                        if (ability is EffectLine continued && abilities.Count > 0 && reminders[^1] == reminder && Continue(abilities[^1], continued.Effect) is { } merged)
                        {
                            ctx.Record(merged, glyph);
                            abilities[^1] = merged;
                            continue;
                        }
                        // "Flip a coin. When you win the flip, copy that spell." "Regenerate it. When it regenerates this way, …"
                        // A "when" on what the ability before it just did is part of that ability, not a state trigger.
                        if (ability is TriggeredAbility { Trigger: ConditionEvent { Condition: OutcomeCondition or WasAffectedThisWay } reflexive, If: null } whenDone
                            && abilities.Count > 0 && reminders[^1] == reminder && Continue(abilities[^1], new Conditional(reflexive.Condition, whenDone.Effect)) is { } joined)
                        {
                            ctx.Record(joined, glyph);
                            abilities[^1] = joined;
                            continue;
                        }
                        ctx.Record(ability, glyph);
                        abilities.Add(ability);
                        reminders.Add(reminder);
                    }
                    continue;
            }

        var opaque = abilities.SelectMany(OpaqueNodes.In).Select(x => x.Glyph).ToList();
        var open = abilities.SelectMany(OpenSlots.In).ToList();
        return (new LineRules(line.SourceText.LineIndex, line.SourceText.FormattedText, abilities, opaque, open), new LineOrigins(ctx.Origins, ctx.UnboundReferences));
    }

}

/// <summary>Where one line's nodes came from in its text (see <see cref="LoweringContext.Origins"/>), and how many references stayed unbound.</summary>
public sealed record LineOrigins(IReadOnlyDictionary<IrNode, Glyphotype.Lowering.SourceSpan> Spans, int UnboundReferences);
