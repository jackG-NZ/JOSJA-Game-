namespace JOSJA_Game
{
    internal class Program
    {
        static void Main()
        {
            /*EXPLANATION: This method is a simple WASD movement system. It continuously checks for user input and changes their position accordingly. Programmed by Jack.*/

            //center position while windowed is 60, 14
            int x = 60, y = 14;

            char movement = 'z';

            bool isMoving = true;

            while (isMoving)
            {
                Console.WriteLine($"X: {x} | Y: {y} | Enter 'e' to stop moving.");
                Console.SetCursorPosition(x, y);
                Console.Write("X");

                //this block of code handles user input
                Console.SetCursorPosition(0, 27);
                Console.Write("WASD: ");
                string input = Console.ReadLine().ToLower();

                //this if statement prevents a crash from happening if the user inputs nothing
                if (input.Length > 0)
                {
                    movement = Convert.ToChar(input.Substring(0, 1));
                }

                switch (movement)
                {
                    case 'w':
                        y -= 1;
                        break;
                    case 'a':
                        x -= 1;
                        break;
                    case 's':
                        y += 1;
                        break;
                    case 'd':
                        x += 1;
                        break;
                    case 'e':
                        isMoving = false;
                        break;
                    default:
                        break;
                }

                //clear the screen to prevent trailing
                Console.Clear();
            }

       
            string currentRoom = "startRoom";
            var rooms = new Dictionary<string, Dictionary <string, string>> //a dictionary containing dictionaries, the first dictionary is the room name, the second dictionary is the direction and the room the direction leads you to
            {
                {
                    "startRoom", new Dictionary<string, string>
                    {
                        { "North", "topRoom" },
                        { "South", "bottomRoom" },
                        { "East", "rightRoom" },
                    }

                },
                {
                    "topRoom", new Dictionary<string, string>
                    {
                        { "South", "startRoom" },
                    }
                },
                {
                    "rightRoom", new Dictionary<string, string>
                    {
                        { "West", "startRoom" }
                    }
                },
                {
                    "bottomRoom", new Dictionary<string, string>
                    {
                        { "North", "startRoom" }
                    }
                }
            };
            //
            Console.WriteLine("Hello, World!");
            while (true)//i'll change this later i swear
            {
                Console.WriteLine($"You are in {currentRoom}. What direction: (North, South, East, West)");
                string input = Console.ReadLine();
                if (rooms[currentRoom].ContainsKey(input))
                {
                    currentRoom = rooms[currentRoom][input];
                }
                else
                {
                    Console.WriteLine("Not that way");
                }
            }
        }
    }
}
