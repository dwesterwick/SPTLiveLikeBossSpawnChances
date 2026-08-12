using LiveLikeBossSpawnChances.Server.Internal;
using LiveLikeBossSpawnChances.Utils;
using SPTarkov.Server.Core.Helpers.Profile;
using SPTarkov.Server.Core.Helpers.Server;
using SPTarkov.Server.Core.Loaders;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Spt.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveLikeBossSpawnChances.Server
{
    public class SpawnAdjustmentsTests
    {
        private MockLogger<LiveLikeBossSpawnChances_Server> _logger;
        private LoggingUtil _loggingUtil;
        private MockConfigUtil _configUtil;

        private ModHelper _modHelper = null!;
        private LocationTable _locationTable = null!;
        private ProfileHelper _profileHelper = null!;

        private ProfileUtil _profileUtil;
        private AdjustSpawnChancesUtil _adjustSpawnChancesUtil;

        [SetUp]
        public void Setup()
        {
            _logger = new MockLogger<LiveLikeBossSpawnChances_Server>();

            RunFromSptInstallDirectoryService.RunFromSptInstallDirectory(LoadSptDependencies);

            _configUtil = new MockConfigUtil(_modHelper);
            _loggingUtil = new LoggingUtil(_logger, _configUtil);

            _profileUtil = new ProfileUtil(_profileHelper);
            _adjustSpawnChancesUtil = new AdjustSpawnChancesUtil(_loggingUtil, _configUtil, _locationTable, _profileUtil);
        }

        [Test]
        public void EnsureSpawnAdjustmentsAreRevsersible()
        {
            Location? customs = _locationTable.GetLocation("bigmap");
            Assert.NotNull(customs, "Could not find Customs location in SPT database");

            string reshalaName = "bossBully";
            BossLocationSpawn? reshala = customs.Base.BossLocationSpawn.FirstOrDefault(boss => boss.BossName == reshalaName);
            Assert.That(reshalaName, Is.EqualTo(reshala?.BossName ?? ""), "Could not find Reshala spawn in Customs");

            double baseValue = 22;
            reshala!.BossChance = baseValue;

            double adjustment = 2;
            _adjustSpawnChancesUtil.AdjustAllBossSpawnChances(adjustment);
            Assert.That(baseValue * adjustment, Is.EqualTo(reshala!.BossChance));

            _adjustSpawnChancesUtil.AdjustAllBossSpawnChances(1);
            Assert.That(baseValue, Is.EqualTo(reshala!.BossChance));
        }

        private void LoadSptDependencies()
        {
            var locales = ProgramHelpers.CreateEarlyLocaleTable() ?? throw new InvalidOperationException("Locales aren't loaded lmao");
            var configuration = ConfigLoader.Initialize(_logger).GetAwaiter().GetResult();
            _modHelper = DI.GetInstance(configuration, locales, _logger).GetService<ModHelper>();
            _locationTable = DI.GetInstance(configuration, locales, _logger).GetService<LocationTable>();
            _profileHelper = DI.GetInstance(configuration, locales, _logger).GetService<ProfileHelper>();
        }
    }
}
