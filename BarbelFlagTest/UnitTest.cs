using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BarbelFlag;

/*
TODO
--------------------------------------------------
refactoring: default setting for GlobalSetting members

init game instance
instantiate chracters with teams
instantiate flags
assign characters to teams to limit (fixed number)
limit handling character actions until the game starts
start game 
handle events (move, attack, capture)
assign score to team when flag is captured
finish game when score reached limit


DONE
--------------------------------------------------
* team
* - capture flags 
* flags
* - status: neutral / capturing  / captured
* - generate score
* character
* - basic stats
* refactoring: fix team ids to 2 only. no need to variable ids
*/

namespace CoreTest
{
    [TestClass]
    public class BarbelFlagTestFarm
    {
        private GlobalSetting globalSetting;

        [TestInitialize]
        public void SetUp()
        {
            globalSetting = new GlobalSetting();
        }

        [TestMethod]
        public void Test0InitTeam()
        {
            Assert.AreEqual(globalSetting.WinScore, 1000);

            var team = new Team(new Team.Initializer
            {
                Setting = globalSetting
            });
        }

        [TestMethod]
        public void Test1NotifyGameInstanceFromTeam()
        {
            var globalSetting = new GlobalSetting();

            var gameInstance = new GameInstance();
            var team = new Team(new Team.Initializer
            {
                Setting = globalSetting,
                Game = gameInstance
            });

            Assert.AreEqual(gameInstance.Status, GameStatus.Initial);
            team.RaiseScoreDummy();
            Assert.AreEqual(gameInstance.Status, GameStatus.End);
        }

        [TestMethod]
        public void Test20InitFlag()
        {
            var flag = new Flag();

            Assert.AreEqual(flag.OwnerTeamFaction, TeamFaction.None);
            Assert.AreEqual(flag.CaptureStatus, Flag.FlagCaptureStatus.Initial);
        }

        [TestMethod]
        public void Test21TeamCaptureAFlag()
        {
            var dummyFlag = new Flag();
            var dummyTeam = new Team(new Team.Initializer
            {
                Faction = TeamFaction.Ciri
            });

            dummyTeam.StartCapture(dummyFlag);
            Assert.AreEqual(dummyFlag.OwnerTeamFaction, TeamFaction.None);
            Assert.AreEqual(dummyFlag.CaptureStatus, Flag.FlagCaptureStatus.Capturing);

            dummyTeam.DoneCapture(dummyFlag);
            Assert.AreEqual(dummyFlag.OwnerTeamFaction, dummyTeam.Faction);
            Assert.AreEqual(dummyFlag.CaptureStatus, Flag.FlagCaptureStatus.Captured);
        }

        [TestMethod]
        public void Test22DifferentTeamsCaptureFlags()
        {
            var team1 = new Team(new Team.Initializer
            {
                Faction = TeamFaction.Ciri
            });

            var team2 = new Team(new Team.Initializer
            {
                Faction = TeamFaction.Eredin
            });

            var flag1 = new Flag();
            var flag2 = new Flag();

            team1.StartCapture(flag1);
            team1.DoneCapture(flag1);

            team2.StartCapture(flag2);
            team2.DoneCapture(flag2);

            Assert.AreEqual(flag1.OwnerTeamFaction, team1.Faction);
            Assert.AreEqual(flag2.OwnerTeamFaction, team2.Faction);
        }

        [TestMethod]
        public void Test23GetScoreFromFlag()
        {
            var flag1 = new Flag();
            var team1 = new Team(new Team.Initializer
            {
                Faction = TeamFaction.Eredin
            });

            team1.StartCapture(flag1);
            team1.DoneCapture(flag1);

            Assert.AreEqual(flag1.Score, 0);

            flag1.GenScore();

            Assert.AreEqual(flag1.Score, 10);
        }

        //[TestMethod]
        //public void Test40CreateMessageQueue()
        //{
        //    MessageBase messageCaptureFlagStart = new MessageCaptureFlagStart();
        //}
    }

    [TestClass]
    public class CharacterTest
    {
        [TestMethod]
        public void Test10CharacterMilli()
        {
            CharacterBase milli = new CharacterMilli();

            Assert.AreEqual(milli.Health, 150);
            Assert.AreEqual(milli.AutoRange, 20);
        }

        [TestMethod]
        public void Test11CharacterEnnfi()
        {
            CharacterBase ennfi = new CharacterEnnfi();

            Assert.AreEqual(ennfi.Health, 100);
            Assert.AreEqual(ennfi.AutoRange, 30);
        }

        [TestMethod]
        public void Test12CharacterInnfi()
        {
            CharacterBase innfi = new CharacterInnfi();

            Assert.AreEqual(innfi.Health, 200);
            Assert.AreEqual(innfi.AutoRange, 20);
        }

        [TestMethod]
        public void Test20CharacterFactory()
        {
            //TODO: load basic character stats by factory 
        }
    }

    [TestClass]
    public class GameInstanceTest
    {
        protected GameInstance game;

        [TestInitialize]
        public void SetUp()
        {
            game = new GameInstance();
        }

        [TestMethod]
        public void Test1InitCharacter()
        {
            var message = new MessageInitCharacter
            {
                UserId = 1,
                CharType = CharacterType.Innfi,
                Faction = TeamFaction.Ciri
            };

            var answer = game.HandleMessage(message);

            Assert.AreEqual(answer.MsgType, message.MsgType);
            Assert.AreEqual(answer.Code, ErrorCode.Ok);

            var answerInitCharacter = (AnswerInitCharacter)answer;

            Assert.AreEqual(message.UserId, answerInitCharacter.UserId);
            Assert.AreEqual(message.Faction, answerInitCharacter.Faction);

            var character = answerInitCharacter.Character;
            Assert.AreEqual(character.CharType, CharacterType.Innfi);
        }

        [TestMethod]
        public void Test1InitCharacter1DuplicateUserId()
        {
            var message = new MessageInitCharacter
            {
                UserId = 1,
                CharType = CharacterType.Innfi,
                Faction = TeamFaction.Eredin
            };

            var answer = game.HandleMessage(message);

            Assert.AreEqual(answer.MsgType, message.MsgType);
            Assert.AreEqual(answer.Code, ErrorCode.Ok);

            var invalidAnswer = game.HandleMessage(message);

            Assert.AreEqual(invalidAnswer.MsgType, message.MsgType);
            Assert.AreEqual(invalidAnswer.Code, ErrorCode.UserAlreadyRegistered);
        }

        [TestMethod]
        public void Test1InitCharacter2CheckTeam()
        {
            var message1 = new MessageInitCharacter
            {
                UserId = 1,
                CharType = CharacterType.Innfi,
                Faction = TeamFaction.Ciri
            };
            var message2 = new MessageInitCharacter
            {
                UserId = 2,
                CharType = CharacterType.Innfi,
                Faction = TeamFaction.Ciri
            };
            game.HandleMessage(message1);
            game.HandleMessage(message2);

            var answerLoadTeam = (AnswerLoadTeam)game.HandleMessage(new MessageLoadTeam
            {
                Faction = TeamFaction.Ciri
            });
            Assert.AreEqual(answerLoadTeam.Code, ErrorCode.Ok);
            Assert.AreEqual(answerLoadTeam.MsgType, MessageType.LoadTeam);
            var members = answerLoadTeam.TeamMembers;

            members.TryGetValue(message1.UserId, out var character1);
            members.TryGetValue(message2.UserId, out var character2);
            Assert.AreEqual(character1.CharType, message1.CharType);
            Assert.AreEqual(character2.CharType, message2.CharType);


            var emptyAnswer = (AnswerLoadTeam)game.HandleMessage(new MessageLoadTeam
            {
                Faction = TeamFaction.Eredin
            });
            Assert.AreEqual(emptyAnswer.TeamMembers.ContainsKey(message1.UserId), false);
            Assert.AreEqual(emptyAnswer.TeamMembers.ContainsKey(message2.UserId), false);
        }

        [TestMethod]
        public void Test1InitCharacter3TeamMemberFull()
        {
            var globalSetting = new GlobalSetting
            {
                MemberCount = 5
            };

            var otherGame = new GameInstance(globalSetting);

            var faction = TeamFaction.Ciri;
            var messages = GenerateDummyMsgInitChar(faction);

            foreach (var message in messages)
            {
                var answer = otherGame.HandleMessage(message);
                Assert.AreEqual(answer.Code, ErrorCode.Ok);
            }

            var invalidMessage = new MessageInitCharacter
            {
                UserId = 99,
                CharType = CharacterType.Innfi,
                Faction = faction
            };

            var invalidAnswer = otherGame.HandleMessage(invalidMessage);
            Assert.AreEqual(invalidAnswer.Code, ErrorCode.TeamMemberCountLimit);
        }

        protected List<MessageInitCharacter> GenerateDummyMsgInitChar(TeamFaction faction)
        {
            var messages = new List<MessageInitCharacter>();

            for (int i = 1; i < 6; i++)
            {
                messages.Add(new MessageInitCharacter
                {
                    UserId = i,
                    CharType = CharacterType.Ennfi,
                    Faction = faction
                });
            }

            return messages;
        }

        [TestMethod]
        public void Test1InitFlags1Instantiate()
        {
            var answer = game.HandleMessage(new MessageGetFlagsStatus());
            Assert.AreEqual(answer.MsgType, MessageType.GetFlagsStatus);

            var answerFlagsStatus = (AnswerGetFlagsStatus)answer;
            Assert.AreEqual(answerFlagsStatus.Flags != null, true);

            foreach (var flag in answerFlagsStatus.Flags)
            {
                Assert.AreEqual(flag.OwnerTeamFaction, TeamFaction.None);
            }
        }

        //[TestMethod]
        //public void Test1InitFlags2IsCorrectStatus()
        //{

        //}
    }
}
