using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarbelFlag
{
    public enum CharacterType
    {
        Innfi = 1,
        Ennfi = 2,
        Milli = 3,
    }

    public abstract class CharacterBase
    {
        public CharacterType CharType { get; protected set; }
        public ObjectPosition Pos { get; set; }

        public int MoveSpeed;
        public int Health;
        public int AutoRange;
        public int AutoDamage;
        public float AutoSpeed;
    }

    public class CharacterMilli : CharacterBase
    {
        public CharacterMilli()
        {
            CharType = CharacterType.Milli;
            MoveSpeed = 30;
            Health = 150;
            AutoRange = 20;
            AutoDamage = 10;
            AutoSpeed = 0.5f;
        }
    }

    public class CharacterEnnfi : CharacterBase
    {
        public CharacterEnnfi()
        {
            CharType = CharacterType.Ennfi;
            MoveSpeed = 35;
            Health = 100;
            AutoRange = 30;
            AutoDamage = 10;
            AutoSpeed = 0.7f;
        }
    }

    public class CharacterInnfi : CharacterBase
    {
        public CharacterInnfi()
        {
            CharType = CharacterType.Innfi;
            MoveSpeed = 25;
            Health = 200;
            AutoRange = 20;
            AutoDamage = 15;
            AutoSpeed = 0.6f;
            Pos = new ObjectPosition(0, 0, 0);
        }
    }
}
