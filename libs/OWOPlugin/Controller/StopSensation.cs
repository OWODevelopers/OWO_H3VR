namespace OWOGame.Controller
{
    internal class StopSensation : SendMessage
    {
        GameAuth game = GameAuth.Empty;
        public StopSensation(Client network) : base(network) { }

        public void Execute() => Execute($"{game.id}*STOP");
        public void Configure(GameAuth game) => this.game = game;
    }
}