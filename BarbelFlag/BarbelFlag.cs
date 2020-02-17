using System.Collections.Generic;


namespace BarbelFlag
{
    public enum TeamFaction
    {
        None = 0,
        Ciri = 1,
        Eredin = 2
    };

    public class Team
    {
        public class Initializer
        {
            public GlobalSetting Setting;
            public GameInstance Game;
            public TeamFaction Faction;
        }

        public TeamFaction Faction { get; protected set; }
        public int Score { get; private set; }
        public Dictionary<int, GameClient> Members { get; protected set; }

        protected GlobalSetting globalSetting;
        protected GameInstance gameInstance;


        public Team(Initializer initializer)
        {
            globalSetting = initializer.Setting;
            gameInstance = initializer.Game;
            Faction = initializer.Faction;
            Members = new Dictionary<int, GameClient>();
        }

        public void AddMember(int userId, GameClient client)
        {
            Members.Add(userId, client);
        }

        public void AddScore()
        {
            Score += 10;
        }
    }


    public enum SkillType : int
    {
        EmptySkill = 0,
        JumpToPosition = 1,
        SimpleDamange = 2,
    }

    public class Skill
    {

    };

    public class SkillJumpToPosition 
    {
        public SkillType Type;

        public SkillJumpToPosition()
        {
            Type = SkillType.JumpToPosition;
        }

        public void Invoke(CharacterBase character, ObjectPosition pos)
        {
            character.Pos = pos;
        }
    };

    public class SkillSimpleDamage
    {
        public SkillType Type;

        public SkillSimpleDamage()
        {
            Type = SkillType.SimpleDamange;
        }

        public void Invoke(CharacterBase character, int damage)
        {
            character.Health -= damage;
            if (character.Health <= 0) character.Health = 0;
        }
    }
}
