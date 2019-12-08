using Microsoft.VisualStudio.TestTools.UnitTesting;
using BarbelFlag;

/*
TODO
--------------------------------------------------
Definition:
* game instance
* - instantiate teams
* - instantiate flags 
* - manage team scores / win condition
* - system io
* 
* team
* - capture flags 
* - raise score
* - manage team members 
* 
* flags
* - status: neutral / capturing  / captured
* player
* - basic stats
* - character type
* - skill
* environments
* field


DONE
--------------------------------------------------
*/

namespace BarbelFlagTest
{
    [TestClass]
    public class BarbelFlagTester
    {
        [TestMethod]
        public void Test0InitTeam()
        {
            var globalSetting = new GlobalSetting();
            Assert.AreEqual(globalSetting.WinScore, 2000);

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
    }
}
