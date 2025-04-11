namespace OWOGame
{
    public partial struct Icon
    {
        public static Icon Empty => new Icon("0");
        public static Icon Death => new Icon("Death-0");
        public static Icon Spiders => new Icon("Spider-0");
        public static Icon Weight => new Icon("Weight-0");
        public static Icon Environment => new Icon("Environment-0");
        public static Icon Alert => new Icon("Alert-0");
        public static Icon Victory => new Icon("Victory-0");

        public struct Impact
        {
            const string PREFIX = "Impact-";
            public static Icon Ball => new Icon(PREFIX + 0);
            public static Icon Dart => new Icon(PREFIX + 1);
            public static Icon Punch => new Icon(PREFIX + 2);
            public static Icon Bullet => new Icon(PREFIX + 3);
        }

        public struct Weapon
        {
            const string PREFIX = "Weapon-";
            public static Icon Axe => new Icon(PREFIX + 0);
            public static Icon Dagger => new Icon(PREFIX + 1);
            public static Icon Gun => new Icon(PREFIX + 2);
            public static Icon SubMachineGun => new Icon(PREFIX + 3);
        }
    }
}