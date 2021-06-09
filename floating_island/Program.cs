using System;

namespace floating_island
{
    //When I started this project, I thought
    //"I'll make my code absolute clear and readable, and it won't be like in my previous project"
    //Now I remember that I thought same starting my previous project... 

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
