using GameCore;

namespace Program
{
    public class Program
    {
        private static Game? game;
        public static void Main()
        {
            game = new();
            game.Run();
        }
    }
}