namespace MTGEngine.Core;

/// <summary>A modal effect: "choose one —" and its bullet modes, which the lowering gives as a ChooseModes effect followed by Mode abilities on the next lines.</summary>
public sealed record ModalEffect(string Count, ImmutableList<Mode> Modes) : Effect;

public static class Modal
{
    /// <summary>The abilities with each ChooseModes effect replaced by a <see cref="ModalEffect"/> holding the Mode abilities that follow it.</summary>
    public static IEnumerable<Ability> Rewrite(IEnumerable<Ability> abilities)
    {
        var list = abilities.ToList();
        for (var i = 0; i < list.Count; i++)
        {
            var ability = list[i];
            if (ChooseModesOf(ability) is not { } choose)
            {
                if (ability is not Mode)
                    yield return ability;
                continue;
            }

            var modes = new List<Mode>();
            var j = i + 1;
            while (j < list.Count && list[j] is Mode mode)
            {
                modes.Add(mode);
                j++;
            }
            i = j - 1;

            if (modes.Count == 0)
            {
                yield return ability;
                continue;
            }

            var modal = new ModalEffect(choose.Count, modes.ToImmutableList());
            yield return WithEffect(ability, modal);
        }
    }

    static ChooseModes ChooseModesOf(Ability ability) =>
        EffectOf(ability) switch
        {
            ChooseModes choose => choose,
            Sequence { Steps: [.., ChooseModes choose] } => choose,
            _ => null,
        };

    static Effect EffectOf(Ability ability) =>
        ability switch
        {
            SpellAbility spell => spell.Effect,
            EffectLine line => line.Effect,
            TriggeredAbility trigger => trigger.Effect,
            ActivatedAbility activated => activated.Effect,
            LoyaltyAbility loyalty => loyalty.Effect,
            StaticAbility statics => statics.Effect,
            _ => null,
        };

    static Ability WithEffect(Ability ability, Effect modal)
    {
        Effect Replace(Effect effect) =>
            effect is Sequence { Steps: [.., ChooseModes] } sequence ? new Sequence([.. sequence.Steps.Take(sequence.Steps.Count - 1), modal]) : modal;

        return ability switch
        {
            SpellAbility spell => spell with { Effect = Replace(spell.Effect) },
            EffectLine line => new SpellAbility(Replace(line.Effect)),
            TriggeredAbility trigger => trigger with { Effect = Replace(trigger.Effect) },
            ActivatedAbility activated => activated with { Effect = Replace(activated.Effect) },
            LoyaltyAbility loyalty => loyalty with { Effect = Replace(loyalty.Effect) },
            StaticAbility statics => statics with { Effect = Replace(statics.Effect) },
            _ => ability,
        };
    }
}
