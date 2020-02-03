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

        public void Move(ObjectPosition targetPos)
        {
            var distX = System.Math.Pow(targetPos.PosX - Pos.PosX, 2);
            var distZ = System.Math.Pow(targetPos.PosZ - Pos.PosZ, 2);
            var distance = System.Math.Sqrt(distX + distZ);

            if (distance < MoveSpeed)
            {
                Pos = targetPos;
                return;
            }

            MoveWithSpeed(targetPos);
        }

        public void MoveWithSpeed(ObjectPosition targetPos)
        {
            var angle = (targetPos.PosX - Pos.PosX) / (targetPos.PosZ - Pos.PosZ);

            var preValue = System.Math.Pow(MoveSpeed, 2);
            preValue = preValue / (angle + 1);

            Pos.PosX = System.Math.Sqrt(preValue);
            Pos.PosZ = Pos.PosX * angle;
        }
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

    public class CharacterFactory
    {
        public CharacterBase GenCharacter(CharacterType type)
        {
            switch (type)
            {
                case CharacterType.Innfi:
                    return new CharacterInnfi();
                case CharacterType.Ennfi:
                    return new CharacterEnnfi();
                case CharacterType.Milli:
                    return new CharacterMilli();
                default:
                    return new CharacterInnfi();
            }
        }

        public CharacterBase GenCharacter(MessageInitCharacter msg)
        {
            var character = GenCharacter(msg.CharType);
            var faction = msg.Faction;

            if(faction == TeamFaction.Ciri)
            {
                character.Pos = new ObjectPosition(0, 0, 0);
            }
            else
            {
                character.Pos = new ObjectPosition(999, 999, 999);
            }

            return character;
        }
    }
}
