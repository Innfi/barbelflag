using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BarbelFlag;

/*
TODO
--------------------------------------------------
refactoring: GameInstance
refactoring: HandleMessage to message queue
- async reply answer 

DONE
--------------------------------------------------
character
- basic stats
flags
- status: neutral / capturing  / captured
- generate score
- flag id
- capture flag by ticks
generate score by flag ticks
- alarm score from flag to game instance by message
refactoring: fix team ids to 2 only. no need to variable ids
init game instance
instantiate chracters with teams
instantiate flags
assign characters to teams to limit (fixed number)
handle events (move, attack, capture)
limit handling character actions until the game starts
start game 
finish game when score reached limit
limit flag ticks after game ends
refactoring: flags ticks from GameInstance.Update()
refactoring: flag status view data
organize Message / Answer from character's view
*/

namespace CoreTest
{
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
        protected GlobalSetting globalSetting;

        [TestInitialize]
        public void SetUp()
        {
            game = new GameInstance();
            globalSetting = new GlobalSetting();
        }

        [TestMethod]
        public void Test1InitCharacter()
        {
            var dummyUserId = 1;
            var gameClient = new GameClient(dummyUserId, game.MsgQ);
            game.AddClient(gameClient);

            var message = new MessageInitCharacter
            {
                UserId = dummyUserId,
                CharType = CharacterType.Innfi,
                Faction = TeamFaction.Ciri,
                SenderUserId = dummyUserId
            };

            game.MsgQ.EnqueueMessage(message);
            game.Update();

            var character = gameClient.Character;
            Assert.AreEqual(character.CharType, message.CharType);
        }

        [TestMethod]
        public void Test1InitCharacter1DuplicateUserId()
        {
            var dummyUserId = 1;
            var gameClient = new GameClient(dummyUserId, game.MsgQ);
            game.AddClient(gameClient);

            var message = new MessageInitCharacter
            {
                UserId = 1,
                CharType = CharacterType.Innfi,
                Faction = TeamFaction.Eredin,
                SenderUserId = 1
            };

            game.MsgQ.EnqueueMessage(message);
            game.Update();

            Assert.AreEqual(gameClient.LastAnswer.Code, ErrorCode.Ok);

            game.MsgQ.EnqueueMessage(message);
            game.Update();

            Assert.AreEqual(gameClient.LastAnswer.Code, ErrorCode.UserAlreadyRegistered);
        }

        [TestMethod]
        public void Test1InitCharacter2CheckTeam()
        {
            var gameClient1 = new GameClient(1, game.MsgQ);
            var gameClient2 = new GameClient(2, game.MsgQ);
            game.AddClient(gameClient1);
            game.AddClient(gameClient2);

            var faction = TeamFaction.Ciri;
            gameClient1.SendDummyMessage(new MessageInitCharacter
            {
                UserId = gameClient1.UserId,
                CharType = CharacterType.Innfi,
                Faction = faction,
                SenderUserId = gameClient1.UserId
            });
            gameClient2.SendDummyMessage(new MessageInitCharacter
            {
                UserId = gameClient2.UserId,
                CharType = CharacterType.Ennfi,
                Faction = faction,
                SenderUserId = gameClient2.UserId
            });
            game.Update();


            gameClient1.SendDummyMessage(new MessageLoadTeam
            {
                Faction = faction,
                SenderUserId = gameClient1.UserId
            });
            game.Update();

            var answer = (AnswerLoadTeam)gameClient1.LastAnswer;
            Assert.AreEqual(answer.Code, ErrorCode.Ok);
            Assert.AreEqual(answer.MsgType, MessageType.LoadTeam);
            var members = answer.TeamMembers;

            Assert.AreEqual(members.ContainsKey(gameClient1.UserId), true);
            Assert.AreEqual(members.ContainsKey(gameClient2.UserId), true);
        }

        [TestMethod]
        public void Test1InitCharacter3TeamMemberFull()
        {
            //FIXME
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
            var answer = game.HandleMessage(new MessageGetFlagViews());
            Assert.AreEqual(answer.MsgType, MessageType.GetFlagViews);

            var answerFlagsStatus = (AnswerGetFlagViews)answer;
            Assert.AreEqual(answerFlagsStatus.FlagViews != null, true);

            foreach (var flag in answerFlagsStatus.FlagViews)
            {
                Assert.AreEqual(flag.OwnerTeamFaction, TeamFaction.None);
            }
        }

        [TestMethod]
        public void Test2CaptureFlagByHandleMessage()
        {
            game.Reset();
            game.Start();
            var flags = LoadFlagViews();
            var targetFlag = flags[0];

            var answer = game.HandleMessage(new MessageStartCapture
            {
                FlagId = targetFlag.FlagId
            });

            Assert.AreEqual(answer.MsgType, MessageType.StartCapture);
            Assert.AreEqual(answer.Code, ErrorCode.Ok);

            var resultViews = LoadFlagViews();
            Assert.AreEqual(resultViews[0].CaptureStatus, Flag.FlagCaptureStatus.Capturing);
            //TODO: get AnswerDoneCapture with timer
        }

        protected List<FlagView> LoadFlagViews()
        {
            var answer = (AnswerGetFlagViews)game.HandleMessage(new MessageGetFlagViews());

            return answer.FlagViews;
        }

        [TestMethod]
        public void Test2InitFlags3OwnerTeamID()
        {
            game.Reset();
            game.Start();

            var initCharFromCiri = new MessageInitCharacter
            {
                UserId = 1,
                CharType = CharacterType.Ennfi,
                Faction = TeamFaction.Ciri
            };
            var initCharFromEredin = new MessageInitCharacter
            {
                UserId = 2,
                CharType = CharacterType.Milli,
                Faction = TeamFaction.Eredin
            };

            game.HandleMessage(initCharFromCiri);
            game.HandleMessage(initCharFromEredin);

            var flags = LoadFlagViews();
            var flagIndexCiri = 1;
            var flagIndexEredin = 4;

            game.HandleMessage(new MessageStartCapture
            {
                FlagId = flags[flagIndexCiri].FlagId,
                Faction = initCharFromCiri.Faction
            });
            game.HandleMessage(new MessageStartCapture
            {
                FlagId = flags[flagIndexEredin].FlagId,
                Faction = initCharFromEredin.Faction
            });

            var resultflags = LoadFlagViews();
            Assert.AreEqual(
                resultflags[flagIndexCiri].CaptureStatus, Flag.FlagCaptureStatus.Capturing);
            Assert.AreEqual(
                resultflags[flagIndexCiri].OwnerTeamFaction, TeamFaction.Ciri);

            Assert.AreEqual(
                resultflags[flagIndexEredin].CaptureStatus, Flag.FlagCaptureStatus.Capturing);
            Assert.AreEqual(
                resultflags[flagIndexEredin].OwnerTeamFaction, TeamFaction.Eredin);
        }

        [TestMethod]
        public void Test2DoneCaptureByFlagTicks()
        {
            game.Reset();
            game.Start();
            game.HandleMessage(new MessageInitCharacter
            {
                UserId = 1,
                CharType = CharacterType.Ennfi,
                Faction = TeamFaction.Ciri
            });

            var index = 1;
            game.HandleMessage(new MessageStartCapture
            {
                FlagId = index,
                Faction = TeamFaction.Ciri
            });

            var views = LoadFlagViews();
            Assert.AreEqual(views[index].CaptureStatus, Flag.FlagCaptureStatus.Capturing);
            //TODO: ticks by internal handler
            for (var i = 0; i < 10; i++) game.Update();

            views = LoadFlagViews();
            Assert.AreEqual(views[index].CaptureStatus, Flag.FlagCaptureStatus.Captured);
        }

        [TestMethod]
        public void Test2GetScoreByTicks()
        {
            game.Reset();
            game.Start();
            game.HandleMessage(new MessageInitCharacter
            {
                UserId = 1,
                CharType = CharacterType.Ennfi,
                Faction = TeamFaction.Ciri
            });
            
            var index = 1;
            game.HandleMessage(new MessageStartCapture
            {
                FlagId = index,
                Faction = TeamFaction.Ciri
            });

            var answerLoadTeam = (AnswerLoadTeam)game.HandleMessage(new MessageLoadTeam
            {
                Faction = TeamFaction.Ciri
            });

            Assert.AreEqual(answerLoadTeam.Score, 0);

            for (var i = 0; i < 10; i++) game.Update();

            var views = LoadFlagViews();
            Assert.AreEqual(views[index].CaptureStatus, Flag.FlagCaptureStatus.Captured);
            
            for (var i = 0; i < 10; i++) game.Update();

            answerLoadTeam = (AnswerLoadTeam)game.HandleMessage(new MessageLoadTeam
            {
                Faction = TeamFaction.Ciri
            });
            Assert.AreEqual(answerLoadTeam.Score, 10);
        }

        [TestMethod]
        public void Test3SendAnswerByMessageQueue()
        {
            //game.Reset();

            //game.MsgQ.EnqueueMessage(new MessageInitCharacter
            //{
            //    UserId = 1,
            //    CharType = CharacterType.Ennfi,
            //    Faction = TeamFaction.Ciri
            //});

            //game.Update();

            //var answer = (AnswerLoadTeam)game.HandleMessage(new MessageLoadTeam
            //{
            //    Faction = TeamFaction.Ciri
            //});
            //var members = answer.TeamMembers;

            //Assert.AreEqual(members.Count, 1);
            //Assert.AreEqual(members[1].CharType, CharacterType.Ennfi);
        }

        [TestMethod]
        public void Test3SendRaiseScoreFromFlagToTeam()
        {
            game.Reset();
            game.Start();

            var faction = TeamFaction.Eredin;
            var capturedFlag = GetCapturedFlag(faction);
            Assert.AreEqual(capturedFlag.OwnerTeamFaction, faction);
            Assert.AreEqual(capturedFlag.CaptureStatus, Flag.FlagCaptureStatus.Captured);

            TickToGenerateScore();
            game.Update();

            var answer = (AnswerLoadTeam)game.HandleMessage(new MessageLoadTeam
            {
                Faction = faction
            });

            Assert.AreEqual(answer.Score, 10);
        }

        protected FlagView GetCapturedFlag(TeamFaction faction)
        {
            var flagId = 1;

            game.MsgQ.EnqueueMessage(new MessageStartCapture
            {
                Faction = faction,
                FlagId = flagId
            });

            game.Update();

            for (int i = 0; i < 10; i++) game.Update();

            game.Update();

            var answer = (AnswerGetFlagViews)game.HandleMessage(
                new MessageGetFlagViews());
            return answer.FlagViews[flagId];
        }

        protected void TickToGenerateScore()
        {
            for (int i = 0; i < 10; i++) game.Update();
        }

        [TestMethod]
        public void Test4DenyStartCaptureBeforeGameStart()
        {
            game.Reset();
            Assert.AreEqual(game.Status, GameStatus.Initial);

            var answer = game.HandleMessage(new MessageInitCharacter
            {
                UserId = 1,
                CharType = CharacterType.Milli,
                Faction = TeamFaction.Ciri
            });
            Assert.AreEqual(answer.Code, ErrorCode.Ok);

            var answer2 = game.HandleMessage(new MessageLoadTeam
            {
                Faction = TeamFaction.Ciri
            });
            Assert.AreEqual(answer2.Code, ErrorCode.Ok);

            var msgStartCpature = new MessageStartCapture
            {
                FlagId = 1,
                Faction = TeamFaction.Ciri
            };

            var invalidAnswer = game.HandleMessage(msgStartCpature);
            Assert.AreEqual(invalidAnswer.Code, ErrorCode.GameNotStarted);

            game.Start();
            Assert.AreEqual(game.Status, GameStatus.Started);

            var startAnswer = game.HandleMessage(msgStartCpature);
            Assert.AreEqual(startAnswer.Code, ErrorCode.Ok);
        }

        [TestMethod]
        public void Test5FinishGame()
        {
            game.Reset();
            AssignCharactersToTeams();
            game.Start();
            
            var faction = TeamFaction.Ciri;
            var flagId = 4;
            CaptureFlag(faction, flagId);

            var views = GetFlagViews();
            Assert.AreEqual(views[flagId].CaptureStatus, Flag.FlagCaptureStatus.Captured);

            GenerateScoreToWin(views[flagId], globalSetting);            
            Assert.AreEqual(game.Status, GameStatus.End);
        }

        protected void AssignCharactersToTeams()
        {
            game.MsgQ.EnqueueMessage(new MessageInitCharacter
            {
                UserId = 1,
                CharType = CharacterType.Milli,
                Faction = TeamFaction.Ciri
            });

            game.MsgQ.EnqueueMessage(new MessageInitCharacter
            {
                UserId = 22,
                CharType = CharacterType.Ennfi,
                Faction = TeamFaction.Eredin
            });

            game.Update();
        }

        protected List<FlagView> GetFlagViews()
        {
            var answer = (AnswerGetFlagViews)game.HandleMessage(
                new MessageGetFlagViews());

            return answer.FlagViews;
        }

        protected void CaptureFlag(TeamFaction faction, int flagId)
        {
            game.MsgQ.EnqueueMessage(new MessageStartCapture
            {
                Faction = faction,
                FlagId = flagId
            });
            game.Update();

            for (int i = 0; i < 10; i++) game.Update();
        }

        protected void GenerateScoreToWin(FlagView view, GlobalSetting globalSetting)
        {
            var score = 0;
            var count = 0;
            while (score < globalSetting.WinScore || count < 20)
            {
                TickToGenerateScore();
                game.Update();

                var team = (AnswerLoadTeam)game.HandleMessage(new MessageLoadTeam
                {
                    Faction = view.OwnerTeamFaction
                });

                score = team.Score;
                count++;
            }

            game.Update();
        }

        [TestMethod]
        public void Test5FlagTickBlockedAfterEnd()
        {
            game.Reset();
            AssignCharactersToTeams();
            game.Start();

            var flagIdCiri = 4;
            var flagIdEredin = 2;
            CaptureFlag(TeamFaction.Ciri, flagIdCiri);
            CaptureFlag(TeamFaction.Eredin, flagIdEredin);

            var flags = GetFlagViews();
            GenerateScoreToWin(flags[flagIdCiri], globalSetting);
            Assert.AreEqual(game.Status, GameStatus.End);

            var answerScore = (AnswerLoadTeam)game.HandleMessage(new MessageLoadTeam
            {
                Faction = TeamFaction.Eredin
            });

            Assert.AreEqual(answerScore.Score, 90);

            TickToGenerateScore();
            game.Update();
            var answer = (AnswerLoadTeam)game.HandleMessage(new MessageLoadTeam
            {
                Faction = TeamFaction.Eredin
            });

            Assert.AreEqual(answer.Score, 90);
        }
    }

    [TestClass]
    public class GameClientTest
    {
        [TestMethod]
        public void Test1CharacterEmpty()
        {
            var userId = 1;
            var client = new GameClient(userId, new MessageQueue());

            Assert.AreEqual(client.Character == null, true);
        }

        [TestMethod]
        public void Test1InitCharacterToMessageQueue()
        {
            var game = new GameInstance(new GlobalSetting());

            var userId = 1;
            var client = new GameClient(userId, game.MsgQ);
            game.AddClient(client);

            var msgInitChar = new MessageInitCharacter
            {
                Faction = TeamFaction.Ciri,
                CharType = CharacterType.Innfi,
                UserId = userId,
                SenderUserId = userId
            };

            client.SendDummyMessage(msgInitChar); //FIXME
            game.Update();

            Assert.AreEqual(client.Character.CharType, msgInitChar.CharType);
        }
    }

}
