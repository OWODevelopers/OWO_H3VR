namespace OWOGame.Controller
{
    internal class SendSensation : SendMessage
    {
        long whenLastSensationEnds;
        int lastPriority = -1;
        GameAuth game = GameAuth.Empty;

        public SendSensation(Client network) : base(network)
        {
        }

        public void Execute(Sensation sensation, long currentTimeMs)
        {
            if (lastPriority > sensation.Priority && currentTimeMs < whenLastSensationEnds) return;

            base.Execute($"{game.id}*SENSATION*{sensation}");

            whenLastSensationEnds = currentTimeMs + (int)(sensation.Duration * 1000);
            lastPriority = sensation.Priority;
        }

        public void Configure(GameAuth game) => this.game = game;
        public void ResetPriority() => whenLastSensationEnds = 0;
    }
}