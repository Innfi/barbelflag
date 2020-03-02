using System;


namespace BarbelFlag
{
    public abstract class StatusEffect
    {
        public abstract bool EffectDone { get; }
        public abstract void TakeEffect();
    }

    public class StatusEffectDoT : StatusEffect
    {
        public CharacterBase Target;
        public int TickCount;
        public int CurrentTick;
        public int TickDamage;
        

        public override bool EffectDone
        {
            get { return CurrentTick >= TickCount; }
        }

        public override void TakeEffect()
        {
            if (EffectDone) return;

            Target.DamageHP(TickDamage);
            CurrentTick++;
        }
    }

    public class StatusEffectHoT : StatusEffect
    {
        public CharacterBase Target;
        public int TickCount;
        public int CurrentTick;
        public int TickHeal;
        

        public override bool EffectDone
        {
            get { return CurrentTick >= TickCount; }
        }

        public override void TakeEffect()
        {
            if (EffectDone) return;

            Target.HealHP(TickHeal);
            CurrentTick++;
        }
    }

    public class StatusEffectDamageBuff : StatusEffect
    {
        public CharacterBase[] Targets;
        public int TickCount;
        public int CurrentTick;
        public int BuffAmount;


        public override bool EffectDone
        {
            get { return CurrentTick >= TickCount; }
        }

        public override void TakeEffect()
        {
            if (CurrentTick == 0)
            {
                foreach (var target in Targets)
                {
                    target.AutoDamage += BuffAmount;
                }
            }

            CurrentTick++;

            if (EffectDone)
            {
                foreach (var target in Targets)
                {
                    target.AutoDamage -= BuffAmount;
                }
            }
        }
    }

    public class StatusEffectHPBuff : StatusEffect
    {
        public CharacterBase[] Targets;
        public int TickCount;
        public int CurrentTick;
        public int BuffAmount;

        public override bool EffectDone
        {
            get
            {
                return CurrentTick >= TickCount;
            }
        }

        public override void TakeEffect()
        {
            if (CurrentTick == 0)
            {
                foreach (var target in Targets)
                {
                    target.CurrentHealth += BuffAmount;
                }
            }

            CurrentTick++;

            if (EffectDone)
            {
                foreach (var target in Targets)
                {
                    target.CurrentHealth -= BuffAmount;
                }
            }
        }
    }
}
