import { CommonUtils } from "./CommonUtils";
import modConfig from "../config/config.json";

import type { DependencyContainer } from "tsyringe";
import type { IPreSptLoadMod } from "@spt/models/external/IPreSptLoadMod";
import type { IPostDBLoadMod } from "@spt/models/external/IPostDBLoadMod";
import type { IPostSptLoadMod } from "@spt/models/external/IPostSptLoadMod";

import type { ILogger } from "@spt/models/spt/utils/ILogger";
import type { DatabaseServer } from "@spt/servers/DatabaseServer";
import type { IDatabaseTables } from "@spt/models/spt/server/IDatabaseTables";
import type { LocaleService } from "@spt/services/LocaleService";
import type { ProfileHelper } from "@spt/helpers/ProfileHelper";

import { ILocation } from "@spt/models/eft/common/ILocation";

import type { StaticRouterModService } from "@spt/services/mod/staticRouter/StaticRouterModService";

const modName = "LiveLikeBossSpawnChances";

export interface IMinMax
{
    min: number,
    max: number
}

class LiveLikeBossSpawnChances implements IPreSptLoadMod, IPostSptLoadMod, IPostDBLoadMod
{
    private commonUtils: CommonUtils

    private logger: ILogger;
    private databaseServer: DatabaseServer;
    private databaseTables: IDatabaseTables;
    private localeService: LocaleService;
    private profileHelper: ProfileHelper;

    private lastAdjustmentMultiplierAppied = 1;

    public preSptLoad(container: DependencyContainer): void 
    {
        this.logger = container.resolve<ILogger>("WinstonLogger");
        const staticRouterModService = container.resolve<StaticRouterModService>("StaticRouterModService");
        
        if (!modConfig.enabled)
        {
            return;
        }

        // Game start
        staticRouterModService.registerStaticRouter(`StaticAkiGameStart${modName}`,
            [{
                url: "/client/game/start",
                // biome-ignore lint/suspicious/noExplicitAny: <explanation>
                action: async (url: string, info: any, sessionId: string, output: string) => 
                {
                    this.adjustAllBossSpawnChances(sessionId);
                    return output;
                }
            }], "aki"
        );
    }
	
    public postDBLoad(container: DependencyContainer): void
    {
        this.databaseServer = container.resolve<DatabaseServer>("DatabaseServer");
        this.localeService = container.resolve<LocaleService>("LocaleService");
        this.profileHelper = container.resolve<ProfileHelper>("ProfileHelper");
		
        this.databaseTables = this.databaseServer.getTables();
        
        this.commonUtils = new CommonUtils(this.logger, this.databaseTables, this.localeService);
    }
	
    public postSptLoad(): void
    {
        if (!modConfig.enabled)
        {
            this.commonUtils.logInfo("Mod disabled in config.json", true);
            return;
        }

        // For testing only
        //this.adjustAllBossSpawnChances("dummySessionId");
    }

    private adjustAllBossSpawnChances(sessionId: string): void
    {
        const playerLevel = this.getPlayerLevel(sessionId);
        const playerHours = this.getPlayerHours(sessionId);
        let minProgressionFactor = 1;

        if (playerLevel < modConfig.adjustments_disabled_after_player_level)
        {
            if (modConfig.adjustment_factors.playerLevel)
            {
                const playerLevelProgressionFactor = this.getProgressionFactor(modConfig.thresholds.playerLevel, playerLevel);
                this.commonUtils.logVerboseInfo(`Calculated progression factor of ${CommonUtils.round(playerLevelProgressionFactor, 2)} for player level ${playerLevel}`);

                minProgressionFactor = Math.min(minProgressionFactor, playerLevelProgressionFactor);
            }
            
            if (modConfig.adjustment_factors.playerHours)
            {
                const playerHoursProgressionFactor = this.getProgressionFactor(modConfig.thresholds.playerHours, playerHours);
                this.commonUtils.logVerboseInfo(`Calculated progression factor of ${CommonUtils.round(playerHoursProgressionFactor, 2)} for ${playerHours} player hours`);

                minProgressionFactor = Math.min(minProgressionFactor, playerHoursProgressionFactor);
            }
        }
        else
        {
            this.commonUtils.logVerboseInfo(`Player level is ${playerLevel}, and no adjustments will be made at level ${modConfig.adjustments_disabled_after_player_level}+`);
        }

        const minAdjustmentRange : IMinMax = modConfig.thresholds.adjustmentRange;
        const adjustmentMultiplier = (minAdjustmentRange.max - minAdjustmentRange.min) * minProgressionFactor + minAdjustmentRange.min;

        this.commonUtils.logInfo(`Scaling boss spawn chances to ${Math.round(adjustmentMultiplier * 100)}% of their EFT spawn chances`);

        const correctedAdjustmentMultiplierAppied = adjustmentMultiplier / this.lastAdjustmentMultiplierAppied;

        for (const locationName in this.databaseTables.locations)
        {
            this.adjustBossSpawnChancesForLocation(this.databaseTables.locations[locationName], correctedAdjustmentMultiplierAppied);
        }

        this.lastAdjustmentMultiplierAppied = adjustmentMultiplier;
    }

    private getPlayerLevel(sessionId: string): number
    {
        const profile = this.profileHelper.getPmcProfile(sessionId);
        if (profile === undefined)
        {
            const defaultPlayerLevel = modConfig.debug.defaultPlayerLevel;
            this.commonUtils.logError(`Could not retrieve player profile. Assuming player level is ${defaultPlayerLevel}.`);

            return defaultPlayerLevel;
        }

        return profile.Info?.Level ?? 0;
    }

    private getPlayerHours(sessionId: string): number
    {
        const pmcProfile = this.profileHelper.getPmcProfile(sessionId);
        if (pmcProfile === undefined)
        {
            const defaultPlayerHours = modConfig.debug.defaultPlayerHours;
            this.commonUtils.logError(`Could not retrieve player profile. Assuming player hours are ${defaultPlayerHours}.`);
            
            return defaultPlayerHours;
        }

        const scavProfile = this.profileHelper.getScavProfile(sessionId);
        const pmcSecondsInRaid = pmcProfile.Stats?.Eft?.TotalInGameTime ?? 0;
        const scavSecondsInRaid = scavProfile?.Stats?.Eft?.TotalInGameTime ?? 0;

        return CommonUtils.round((pmcSecondsInRaid + scavSecondsInRaid) / 3600.0, 2);
    }

    private getProgressionFactor(progressionRange: IMinMax, value: number): number
    {
        const progressionFraction = (value - progressionRange.min) / (progressionRange.max - progressionRange.min);
        const progressionFactor = CommonUtils.interpolateForFirstCol(modConfig.chance_progression_rate, progressionFraction);

        return progressionFactor;
    }

    private adjustBossSpawnChancesForLocation(location: ILocation, adjustmentFactor: number): void
    {
        if ((location.base === undefined) || (location.base.BossLocationSpawn === undefined))
        {
            return;
        }

        for (const bossSpawn of location.base.BossLocationSpawn)
        {
            if (location.base.Name in modConfig.ignored_bosses)
            {
                if (modConfig.ignored_bosses[location.base.Name].includes(bossSpawn.BossName))
                {
                    continue;
                }
            }

            const originalChance = bossSpawn.BossChance;

            if (modConfig.blocked_bosses.includes(bossSpawn.BossName))
            {
                bossSpawn.BossChance = 0;
            }
            else
            {
                bossSpawn.BossChance = Math.max(0, Math.min(100, Math.round(originalChance * adjustmentFactor)));
            }

            if (originalChance !== bossSpawn.BossChance)
            {
                this.commonUtils.logVerboseInfo(`Changed spawn chance for ${bossSpawn.BossName} on ${location.base.Name} from ${originalChance}% to ${bossSpawn.BossChance}%`);
            }
        }
    }
}

module.exports = { mod: new LiveLikeBossSpawnChances() }