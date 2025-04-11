using System.Threading.Tasks;
using OWOGame.Controller;
using OWOGame.Infraestructure;

namespace OWOGame
{
    public class OWO
    {
        static OWO instance;
        static OWO Instance => instance ?? (instance = new OWO(ClientFactory.Create(new UDPNetwork()), new RealTimeClock()));
        public static ConnectionState ConnectionState => Instance.client.State;

        /// <summary>
        /// Returns all the discovered OWO apps.
        /// </summary>
        public static string[] DiscoveredApps => Instance.client.DiscoveredServers.ToArray();
 
        readonly Client client;
        readonly SendSensation send;
        readonly StopSensation stop;
        readonly Disconnect disconnect;
        readonly Connect connect;
        readonly RealTimeClock clock;

        internal OWO(Client client,RealTimeClock clock)
        {
            this.client = client;
            send = new SendSensation(client);
            stop = new StopSensation(client);
            connect = new Connect(client);
            disconnect = new Disconnect(client);
            this.clock = clock;
        }

        /// <summary>
        /// Assigns a GameAuth file that will be used to authenticate with the owo app.
        /// </summary>
        /// <param name="game"></param>
        public static void Configure(GameAuth game) => Instance.ConfigureB(game);

        internal void ConfigureB(GameAuth game)
        {
            connect.Configure(game);
            send.Configure(game);
            stop.Configure(game);
        }

        /// <summary>
        /// Searches nearby owo apps to connect.
        /// </summary>
        /// <returns></returns>
        public static Task AutoConnect() => Instance.AutoConnectB();
        internal Task AutoConnectB() => Task.Run(connect.AutoConnect);

        /// <summary>
        /// Searches for nearby OWO apps and stores them in the DiscoveredApps property.
        /// </summary>
        /// <returns></returns>
        public static void StartScan() => Instance.StartScanB();
        internal void StartScanB() => Task.Run(connect.ScanServer);

        /// <summary>
        /// Connects to a list of specific owo apps.
        /// </summary>
        /// <returns></returns>
        public static Task Connect(params string[] ips) => Instance.ConnectB(ips);
        internal Task ConnectB(params string[] ips) => Task.Run(() => connect.ManualConnect(ips));

        /// <summary>
        /// Stops the current sensation.
        /// </summary>
        /// <returns></returns>
        public static void Stop() => Instance.StopB();

        internal void StopB()
        {
            stop.Execute();
            send.ResetPriority();
        }

        /// <summary>
        /// Sends a sensation to the connected owo app.
        /// </summary>
        /// <param name="sensation"></param>
        /// <param name="muscles"></param>
        public static void Send(Sensation sensation, params Muscle[] muscles) => Instance.SendB(sensation, muscles);

        internal void SendB(Sensation sensation, params Muscle[] muscles)
        {
            send.Execute(sensation.WithMuscles(muscles), clock.TotalMilliseconds); 
        }   

        /// <summary>
        /// Disconnects from the connected owo app.
        /// </summary>
        public static void Disconnect() => Instance.DisconnectB();

        internal void DisconnectB() => disconnect.Execute();
    }
}