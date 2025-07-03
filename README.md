Simulates boss spawn chances gradually increasing throughout a "wipe" based on your player level and in-raid hours.

Instead of having late-wipe boss spawn chances causing your regular death during a new playthough, this mod will keep spawn chances closer to BSG's early-wipe values. After you reach a certain player level and in-raid hours (combined between PMC and Scav profiles), boss chances will gradually start increasing. After you reach a certain player level and in-raid hours, boss chances will be left at their (default) late-wipe values. However, some bosses will always have the same spawn chances, such as Raiders on Labs and Rogues on Lighthouse. Also, other "event" boss spawns will be eliminated such as Bloodhounds and "crazy" Scavs. 

Bosses (that aren't ignored or blocked from spawning) will have their spawn chances scaled using the following system:
1) The overall range that spawn chances are allowed to be scaled is 20%->100% by default. For example, if you're starting a new profile and Reshala normally spawns at 30%, he will instead spawn at 20% of that value (6%). Later in your "wipe", he will return to spawning at 30% again.
2) Once you're above a certain player level AND above a certain number of in-raid hours, boss spawn chances will start gradually increasing until you hit an upper threshold for both player level AND in-raid hours. After this point, boss spawn chances will remain at their vanilla SPT (late-wipe) values. By default, spawn-rate scaling is linear between the lower and upper thresholds, but this can be adjusted.
3) After you exceed an even higher player level (40 by default), boss spawn chances will remain at their vanilla SPT (late-wipe) values regardless of your in-raid hours. This allows SPT Dev accounts to keep vanilla boss spawn rates.
4) Spawn rates will be calculated each time the game is started. As you progress in your "wipe" or switch to another profile, spawn rates will change accordingly. 

This mod is highly configurable and allows the following to be adjusted:
* `adjustment_factors`: whether player level, player in-raid hours, or both will be used to adjust boss spawn chances
* `adjustments_disabled_after_player_level`: the upper threshold described in step (3) above which no spawn-rate scaling will occur.
* `thresholds.adjustmentRange`: the upper and lower scaling amounts that can be applied to boss spawn rates (20% minimum and 100% maximum, relative to vanilla SPT values, by default).
* `thresholds.playerLevel` and `thresholds.playerHours`: The range of player levels and in-raid hours between which boss spawn rates will be scaled. If either value are below their corresponding minimum threshold, boss spawn rates will be scaled by the minimum value in `thresholds.adjustmentRange`. After both values are above their corresponding maximum thresholds, boss spawn rates will be scaled by the maximum value in `thresholds.adjustmentRange`.
* `ignored_bosses`: the bosses on each map that will not have their spawn rates adjusted by this mod. By default, this includes Rogues and Zryachiy on Lighthouse, Raiders on Labs, and Santa on all maps.
* `blocked_bosses`: the bosses that will be blocked from spawning on all maps. By default, this includes Bloodhounds and "crazy" Scavs.
* `chance_progression_rate`: the scaling curve that will be applied when either player level or in-raid hours are between their corresponding ranges defined in `thresholds`. By default, scaling is linear.

**Known Issues:**
* Due to rounding error, spawn rates might not exactly match vanilla SPT values after switching from a new profile to a Dev (or other late-wipe) profile. However, they should be close enough that you won't notice a difference.
* If you're using a multi-player mod, spawn rates will only be set using the host's profile. 
