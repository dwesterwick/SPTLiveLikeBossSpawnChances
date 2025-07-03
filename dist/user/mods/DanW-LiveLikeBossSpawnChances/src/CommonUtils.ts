import modConfig from "../config/config.json";

import type { ILogger } from "@spt/models/spt/utils/ILogger";
import type { IDatabaseTables } from "@spt/models/spt/server/IDatabaseTables";
import type { LocaleService } from "@spt/services/LocaleService";

export class CommonUtils
{
    private debugMessagePrefix = "[Live Like Boss Spawn Chances] ";
    private translations: Record<string, string>;
	
    constructor (private logger: ILogger, private databaseTables: IDatabaseTables, private localeService: LocaleService)
    {
        // Get all translations for the current locale
        this.translations = this.localeService.getLocaleDb();
    }

    public static round(num: number, precision: number): number
    {
        if (Math.round(precision) !== precision)
        {
            throw new Error("precision must be an integer");
        }

        const scalingFactor = Math.pow(10, precision);

        return Math.round(num * scalingFactor) / scalingFactor;
    }

    public logVerboseInfo(message: string): void
    {
        if (!modConfig.debug.verbose_logging)
            return;

        this.logInfo(message);
    }
	
    public logInfo(message: string, alwaysShow = false): void
    {
        if (modConfig.enabled || alwaysShow)
            this.logger.info(this.debugMessagePrefix + message);
    }

    public logWarning(message: string): void
    {
        this.logger.warning(this.debugMessagePrefix + message);
    }

    public logError(message: string): void
    {
        this.logger.error(this.debugMessagePrefix + message);
    }
	
    public getTraderName(traderID: string): string
    {
        const translationKey = `${traderID} Nickname`;
        if (translationKey in this.translations)
            return this.translations[translationKey];
		
        // If a key can't be found in the translations dictionary, fall back to the template data
        const trader = this.databaseTables.traders[traderID];
        return trader.base.nickname;
    }
	
    public getItemName(itemID: string): string
    {
        const translationKey = `${itemID} Name`;
        if (translationKey in this.translations)
            return this.translations[translationKey];
		
        // If a key can't be found in the translations dictionary, fall back to the template data
        const item = this.databaseTables.templates.items[itemID];
        return item._name;
    }

    public static interpolateForFirstCol(array: number[][], value: number): number
    {
        if (array.length === 1)
        {
            return array[array.length - 1][1];
        }

        if (value <= array[0][0])
        {
            return array[0][1];
        }

        for (let i = 1; i < array.length; i++)
        {
            if (array[i][0] >= value)
            {
                if (array[i][0] - array[i - 1][0] === 0)
                {
                    return array[i][1];
                }

                return array[i - 1][1] + (value - array[i - 1][0]) * (array[i][1] - array[i - 1][1]) / (array[i][0] - array[i - 1][0]);
            }
        }

        return array[array.length - 1][1];
    }
}