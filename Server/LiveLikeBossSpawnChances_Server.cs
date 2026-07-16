using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;

namespace LiveLikeBossSpawnChances;

[Injectable(TypePriority = OnLoadOrder.PreSptModLoader + LiveLikeBossSpawnChances_Server.LOAD_ORDER_OFFSET)]
public class LiveLikeBossSpawnChances_Server : IOnLoad
{
    public const int LOAD_ORDER_OFFSET = 1;

    public LiveLikeBossSpawnChances_Server()
    {

    }

    public Task OnLoad()
    {
        return Task.CompletedTask;
    }
}
