using System;

namespace floating_island
{
    //When I started this project, I thought
    //"I'll my code absolute clear and readable, and it won't like in my previous project"
    //Now I remember that I said same starting my previous project... 

    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new Game1())
                game.Run();
        }
    }
}
