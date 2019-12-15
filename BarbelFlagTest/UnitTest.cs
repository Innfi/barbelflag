using Microsoft.VisualStudio.TestTools.UnitTesting;
using BarbelFlag;

/*
TODO
--------------------------------------------------
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

            Assert.AreEqual(flag.OwnerTeamID, 0);
            Assert.AreEqual(flag.CaptureStatus, Flag.FlagCaptureStatus.Initial);
        }

        [TestMethod]
        public void Test21TeamCaptureAFlag()
        {
            var dummyFlag = new Flag();
            var dummyTeamID = 123;
            var dummyTeam = new Team(new Team.Initializer
            {
                TeamID = dummyTeamID
            });

            dummyTeam.StartCapture(dummyFlag);
            Assert.AreEqual(dummyFlag.OwnerTeamID, 0);
            Assert.AreEqual(dummyFlag.CaptureStatus, Flag.FlagCaptureStatus.Capturing);

            dummyTeam.DoneCapture(dummyFlag);
            Assert.AreEqual(dummyFlag.OwnerTeamID, dummyTeamID);
            Assert.AreEqual(dummyFlag.CaptureStatus, Flag.FlagCaptureStatus.Captured);
        }

        [TestMethod]
        public void Test22DifferentTeamsCaptureFlags()
        {
            var teamID1 = 123;
            var team1 = new Team(new Team.Initializer
            {
                TeamID = teamID1
            });

            var teamID2 = 345;
            var team2 = new Team(new Team.Initializer
            {
                TeamID = teamID2
            });

            var flag1 = new Flag();
            var flag2 = new Flag();

            team1.StartCapture(flag1);
            team1.DoneCapture(flag1);

            team2.StartCapture(flag2);
            team2.DoneCapture(flag2);

            Assert.AreEqual(flag1.OwnerTeamID, team1.TeamID);
            Assert.AreEqual(flag2.OwnerTeamID, team2.TeamID);
        }

        [TestMethod]
        public void Test23GetScoreFromFlag()
        {
            var flag1 = new Flag();
            var teamID1 = 123;
            var team1 = new Team(new Team.Initializer
            {
                TeamID = teamID1
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
                TeamId = 0
            };

            var answer = game.HandleMessage(message);

            Assert.AreEqual(answer.MsgType, message.MsgType);
            Assert.AreEqual(answer.Code, ErrorCode.Ok);

            var answerInitCharacter = (AnswerInitCharacter)answer;

            Assert.AreEqual(message.UserId, answerInitCharacter.UserId);
            Assert.AreEqual(message.TeamId, answerInitCharacter.TeamId);

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
                TeamId = 0
            };

            var answer = game.HandleMessage(message);

            Assert.AreEqual(answer.MsgType, message.MsgType);
            Assert.AreEqual(answer.Code, ErrorCode.Ok);

            var invalidAnswer = game.HandleMessage(message);

            Assert.AreEqual(invalidAnswer.MsgType, message.MsgType);
            Assert.AreEqual(invalidAnswer.Code, ErrorCode.UserAlreadyRegistered);
        }

        [TestMethod]
        public void Test1InitCharacter3CheckTeam()
        {
            var teamId = 123;

            var message1 = new MessageInitCharacter
            {
                UserId = 1,
                CharType = CharacterType.Innfi,
                TeamId = teamId
            };
            var message2 = new MessageInitCharacter
            {
                UserId = 2,
                CharType = CharacterType.Innfi,
                TeamId = teamId
            };

            game.HandleMessage(message1);
            game.HandleMessage(message2);

            var messageLoadTeam = new MessageLoadTeam
            {
                TeamId = teamId
            };

            var answer = game.HandleMessage(messageLoadTeam);
            Assert.AreEqual(answer.Code, ErrorCode.Ok);
            Assert.AreEqual(answer.MsgType, MessageType.LoadTeam);

            var answerLoadTeam = (AnswerLoadTeam)answer;
            var teamMembers = answerLoadTeam.TeamMembers;

            var player1Result = teamMembers.TryGetValue(message1.UserId, 
                out var character1);
            Assert.AreEqual(player1Result, true);
            Assert.AreEqual(character1.CharType, message1.CharType);

            var player2Result = teamMembers.TryGetValue(message2.UserId,
                out var character2);
            Assert.AreEqual(player2Result, true);
            Assert.AreEqual(character2.CharType, message1.CharType);
        }

        [TestMethod]
        public void Test1InitCharacter4CheckDifferentTeam()
        {
            var team1Id = 123;
            var message1 = new MessageInitCharacter
            {
                UserId = 1,
                CharType = CharacterType.Innfi,
                TeamId = team1Id
            };

            var team2Id = 422;
            var message2 = new MessageInitCharacter
            {
                UserId = 2,
                CharType = CharacterType.Innfi,
                TeamId = team2Id
            };

            game.HandleMessage(message1);
            game.HandleMessage(message2);

            var answer = game.HandleMessage(new MessageLoadTeam
            {
                TeamId = team1Id
            });
            Assert.AreEqual(answer.Code, ErrorCode.Ok);
            Assert.AreEqual(answer.MsgType, MessageType.LoadTeam);

            var answerLoadTeam = (AnswerLoadTeam)answer;
            var teamMembers = answerLoadTeam.TeamMembers;

            var player1Result = teamMembers.TryGetValue(message1.UserId,
                out var character1);
            Assert.AreEqual(player1Result, true);
            Assert.AreEqual(character1.CharType, message1.CharType);
            Assert.AreEqual(teamMembers.TryGetValue(message2.UserId,
                out var character), false);

            var answer2 = (AnswerLoadTeam)game.HandleMessage(new MessageLoadTeam
            {
                TeamId = team2Id
            });
            Assert.AreEqual(answer.Code, ErrorCode.Ok);
            Assert.AreEqual(answer.MsgType, MessageType.LoadTeam);

            var teamMembers2 = answer2.TeamMembers;
            var player2Result = teamMembers.TryGetValue(message2.UserId,
                out var character2);
            Assert.AreEqual(player2Result, true);
            Assert.AreEqual(character2.CharType, message2.CharType);
        }

        //[TestMethod]
        //public void Test1InitCharacter3DuplicateUserIdOtherTeam()
        //{

        //}
    }
}
