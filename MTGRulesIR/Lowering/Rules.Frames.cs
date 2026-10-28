namespace MTGRulesIR.Lowering;

/// <summary>The ability frames: what a line of card text is - a keyword line, a static, a trigger, an activated ability, a mode - and the sentence-level glue inside their effects.</summary>
public sealed partial class Lowerer
{
    void RegisterFrames()
    {
        Rule<G.SubjectDoes>((g, ctx) =>
        {
            var previous = ctx.Subject;
            ctx.Subject = LowerAs<Selector>(g.Subject, ctx);

            try
            {
                return LowerAs<Effect>(g.Predicate, ctx);
            }
            finally
            {
                ctx.Subject = previous;
            }
        });

        Rule<G.ActivatedAbility>((g, ctx) =>
            new ActivatedAbility(LowerAllAs<Cost>(g.Costs.Items, ctx), LowerAs<Effect>(g.Effect, ctx)));

        Rule<G.LoyaltyAbility>((g, ctx) =>
            new LoyaltyAbility(Signed(g.Cost.Sign ?? G.PlusMinus.Plus, LowerAs<Quantity>(g.Cost.Amount, ctx)), LowerAs<Effect>(g.Effect, ctx)));

        Rule<G.ChapterAbility>((g, ctx) =>
            new ChapterAbility(g.Chapters.Items.Select(x => (int)x + 1).ToList(), LowerAs<Effect>(g.Effect, ctx)));

        Rule<G.Mode>((g, ctx) => new Mode(LowerAs<Effect>(g.Effect, ctx)));

        Rule<G.ChooseOne>((g, ctx) =>
            new ChooseModes(g.Count.ToString(), g.Chooser is { } chooser ? Player(chooser) : null));

        Rule<G.PermanentEventTrigger>((g, ctx) =>
        {
            var subject = LowerAs<Selector>(g.Subject, ctx);
            var under = g.Under is null ? null : LowerAs<Selector>(g.Under.Whose, ctx);
            Condition whileCondition = null;
            string only = null;

            if (g.Period is not null)
            {
                var period = Lower(g.Period, ctx);
                if (period is Condition condition)
                    whileCondition = condition;
                else if (period is not null)
                    only = period is Term term ? term.Value : ctx.TextOf(g.Period);
            }

            Event trigger;

            if (g.Event.GetResolvedValue() is G.CombatEncounter encounter)
                trigger = new CombatEncounterEvent(subject, encounter.Encounter.ToString(), LowerAs<Selector>(encounter.Other, ctx));
            else
            {
                List<IrNode> details = [];
                var events = Terms(g.Event, ctx, details);
                trigger = events.Count == 0 && details.Count > 0
                    ? (Event)Opaque(typeof(Event), ctx.TextOf(g.Event), g.Event.GetResolvedType()?.Name)
                    : new ObjectEvent(subject, events, under, only, whileCondition, OrNull(details));
            }

            return new TriggeredAbility(trigger, LowerAs<Effect>(g.Effect, ctx));
        });

        Rule<G.PutIntoGraveyardEvent>((g, ctx) =>
            new Term($"IsPutInto{g.Whose}Graveyard{g.Origin}", nameof(G.PermanentEvent)));

        Rule<G.PhaseTrigger>((g, ctx) =>
            new TriggeredAbility(PhaseEventOf(g.When, ctx), LowerAs<Effect>(g.Effect, ctx)));

        Rule<G.SpellCastTrigger>((g, ctx) =>
            new TriggeredAbility(
                new SpellCastEvent(
                    Player(g.Caster),
                    g.Narrowed is null ? LowerAs<Selector>(g.Spell, ctx) : Narrow(LowerAs<Selector>(g.Spell, ctx), Lower(g.Narrowed, ctx)),
                    g.Verb == G.CastVerb.CastOrCopy,
                    g.While is null ? null : LowerAs<Condition>(g.While.Condition, ctx)),
                LowerAs<Effect>(g.Effect, ctx)));

        Rule<G.IndefiniteSpell>((g, ctx) =>
            new ObjectSelector(Quantifier.Any, FilterOf(ctx, kinds: null, qualifiers: g.Qualifiers, narrowing: [g.With]) with { Kinds = ["spell"] }, Other: g.Determiner == G.IndefiniteDeterminer.Another));

        Rule<G.TriggeredAbility>((g, ctx) =>
            new TriggeredAbility(LowerAs<Event>(g.Condition, ctx), LowerAs<Effect>(g.Effect, ctx)));

        // "if X would happen, Y" is a replacement effect; any other condition is a conditional one.
        Rule<G.ConditionalEffect>((g, ctx) =>
            Lower(g.Condition, ctx) is WouldHappen would
                ? new Replacement(would, LowerAs<Effect>(g.Effect, ctx))
                : new Conditional(LowerAs<Condition>(g.Condition, ctx), LowerAs<Effect>(g.Effect, ctx)));

        Rule<G.WhileConditionStatic>((g, ctx) =>
            new StaticAbility(LowerAs<Effect>(g.Effect, ctx), LowerAs<Condition>(g.Condition, ctx)));

        Rule<G.AbilityWordAbility>((g, ctx) =>
            new LabeledAbility(g.Word.ToString(), AsAbility(Lower(g.Ability, ctx), ctx, g.Ability)));

        Rule<G.CardAbilityLine>((g, ctx) =>
        {
            List<IrNode> other = [];
            return new KeywordLine(KeywordsOf(g.Items, ctx, other), OrNull(other));
        });

        Rule<G.KeywordWithReminder>((g, ctx) =>
        {
            List<IrNode> other = [];
            List<KeywordInstance> keywords = [];
            if (g.LeadingKeywords is not null)
                keywords.AddRange(KeywordsOf(g.LeadingKeywords.Items.Select(x => x.Keywords.Items).ToList(), ctx, other));
            keywords.AddRange(KeywordsOf(g.Keywords.Items, ctx, other));
            return new KeywordLine(keywords, OrNull(other));
        });

        Rule<G.KeywordsBeforeSemicolon>((g, ctx) => Lower(g.Keywords, ctx));

        // Reminder text restates a rule the engine already has.
        Rule<G.ReminderText>((g, ctx) => null);

        Rule<G.EnchantPermanent>((g, ctx) =>
            new EnchantAbility(new ObjectSelector(Quantifier.Any, FilterOf(ctx, kinds: g.Kind, qualifiers: g.Qualifiers, controller: g.Controller, narrowing: [g.With, g.CardOutsideBattlefield]))));

        Rule<G.ActivationRestriction>((g, ctx) =>
            new ActivationLimits(LowerAll(g.Timings, ctx).Select(x => x as Timing ?? new Timing("Other", Detail: x)).ToList()));

        // A one-of whose alternatives include plain bools, so it's read property by property rather than by its resolved value.
        Rule<G.ActivationTiming>((g, ctx) =>
            g.During is not null ? new Timing("During", PhaseRefOf(g.During, ctx), Detail: PhaseRefOf(g.During, ctx) is null ? Lower(g.During, ctx) : null)
            : g.Before is not null ? new Timing("Before", Detail: Lower(g.Before, ctx))
            : g.OnceEachTurn ? new Timing("OnceEachTurn")
            : g.Once ? new Timing("Once")
            : g.AsASorcery ? new Timing("SorcerySpeed")
            : g.AsAnInstant ? new Timing("InstantSpeed")
            : g.OnlyIf is not null ? new Timing("If", If: LowerAs<Condition>(g.OnlyIf, ctx))
            : g.OnlyIfState is not null ? new Timing("If", If: LowerAs<Condition>(g.OnlyIfState, ctx))
            : new Timing("Other", Detail: new OpaqueNode(ctx.TextOf(g), nameof(G.ActivationTiming))));

        Rule<G.DelayedEffect>((g, ctx) =>
            new Delayed(PhaseEventOf(g.When, ctx), LowerAs<Effect>(g.Effect, ctx)));

        Rule<G.ThenEffect>((g, ctx) => Lower(g.Effect, ctx));

        Rule<G.MayDo>((g, ctx) => new Optional(LowerAs<Effect>(g.Predicate, ctx), ctx.Subject));
    }

    /// <summary>A selector narrowed by a further clause ("that targets this creature"), kept under its filter's <see cref="ObjectFilter.With"/>.</summary>
    static Selector Narrow(Selector selector, IrNode narrowing) =>
        narrowing is null ? selector
        : selector is ObjectSelector objects
            ? objects with { Filter = (objects.Filter ?? new ObjectFilter()) with { With = [.. objects.Filter?.With ?? [], narrowing] } }
            : selector;
}
