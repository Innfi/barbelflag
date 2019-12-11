using Microsoft.VisualStudio.TestTools.UnitTesting;
using BarbelFlag;

/*
TODO
--------------------------------------------------
init game instance
instantiate chracters
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
*/

namespace BarbelFlagTest
{
    [TestClass]
    public class BarbelFlagTester
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

        [TestMethod]
        public void Test24GetScoreFromFlagOverTime()
        {

        }
    }
}
