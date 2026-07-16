using SPTarkov.Server.Core.DI;

namespace DansDevTools.Routers.Internal
{
    public interface IRouteInfo
    {
        public string Name { get; }
        public IRouteHandler Handler { get; }
        public string Path { get; }
        public RouteAction? Action { get; }
    }
}
