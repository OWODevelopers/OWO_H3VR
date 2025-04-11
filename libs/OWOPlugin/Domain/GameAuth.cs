namespace OWOGame
{
    public class GameAuth
    {
        public readonly string id;
        public readonly BakedSensation[] sensations = new BakedSensation[0];

        internal GameAuth() { }
        internal GameAuth(string id, params BakedSensation[] sensations)
        {
            this.id = id;
            this.sensations = sensations;
        }

        public GameAuth WithId(string id) => new GameAuth(id, sensations);

        public static GameAuth Parse(string auth) => auth;
        public static GameAuth Create(params BakedSensation[] sensations) => new GameAuth("0", sensations);
        public override string ToString() => this;

        public static implicit operator GameAuth(string auth) => GamesParser.From(auth);
        public static implicit operator string(GameAuth auth) => GamesBuilder.Build(auth);
        public static GameAuth Empty => GameAuth.Create().WithId("0");
    }
}