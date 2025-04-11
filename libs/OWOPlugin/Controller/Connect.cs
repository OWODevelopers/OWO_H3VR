using System.Threading.Tasks;

namespace OWOGame.Controller
{
    internal class Connect
    {
        GameAuth game = GameAuth.Empty;

        readonly Client client;
        public Connect(Client client) => this.client = client;

        public Task ScanServer() => client.ScanServer();
        public Task AutoConnect() => client.FindServer($"{game.id}*AUTH*{game}", "255.255.255.255");
        public Task ManualConnect(params string[] ips) => client.FindServer($"{game.id}*AUTH*{game}", ips);
        public void Configure(GameAuth game) => this.game = game;
    }
}