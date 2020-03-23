using System;


namespace BarbelFlag
{
    public abstract class StatusEffect
    {
        public string EffectName;

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
            if (EffectDone) return;

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

    public class EffectDescription
    {
        public string EffectName = "StatusEffectDoT";
        public int TickCount = 10;
        public int EffectAmount = 12;
        public string TargetEntity = "CharacterBase";
        public string TargetStat = "Health";
        public string Action = "Increase";

        public override bool Equals(object obj)
        {
            if ((obj == null)) return false;
            if (!this.GetType().Equals(obj.GetType())) return false;

            var rhs = (EffectDescription)obj;

            if (EffectName != rhs.EffectName) return false;
            if (TickCount != rhs.TickCount) return false;
            if (EffectAmount != rhs.EffectAmount) return false;
            if (TargetEntity != rhs.TargetEntity) return false;
            if (TargetStat != rhs.TargetStat) return false;
            if (Action != rhs.Action) return false;

            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class StatusEffectParser
    {
        public bool Parse(string description, out StatusEffect effect)
        {
            throw new NotImplementedException();
        }
    }
}
