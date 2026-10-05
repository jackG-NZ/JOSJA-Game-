namespace JOSJA_Game
{
    internal class Program
    {
        static string currentLevel = "levelOne";

        static void Main()
        {
            GameIntro();

            while (true) //will eventually add some sort of condition here
            {
                PlayerMovement();
            }

            Console.ReadLine();
        }

        static void GameIntro()
        {
            /*EXPLANATION: This method serves as the title screen. We should make it look nice towards the end of the project*/

            char skipDialogue = 'n'; //set this to n, so if the user enters nothing it assumes they don't want to skip the intro dialogue

            Console.SetCursorPosition(23, 14);
            Console.WriteLine("JOSJA Game Prototype. WORKS BEST IN WINDOWED MODE. Press [ENTER] to proceed.");
            Console.ReadLine();
            Console.Clear();

            Console.SetCursorPosition(35, 14);
            Console.WriteLine("Would you like to skip the opening dialogue? (y|n): ");
            string input = Console.ReadLine().ToLower();

            //this if statement prevents a crash from happening if the user inputs nothing
            if (input.Length > 0)
            {
                skipDialogue = Convert.ToChar(input.Substring(0, 1));
            }

            Console.Clear();

            if (skipDialogue == 'n')
            {
                IntroDialogue(); //COMMENT OUT THIS METHOD TO SKIP THE INTRO DIALOGUE
            }
        }

        static void IntroDialogue()
        {
            /*EXPLANATION: This method gives the player a brief intro to the game's story*/

            int dialogueTimer = 4250; //this is the time each line of text will be displayed on the screen in ms
            string playerName = "Player";

            Console.ForegroundColor = ConsoleColor.Red;

            Console.SetCursorPosition(58, 14);
            Console.WriteLine("Hello.");
            Thread.Sleep(dialogueTimer);
            Console.Clear();

            Console.SetCursorPosition(47, 14);
            Console.WriteLine("I am the Demon Abezethibou.");
            Thread.Sleep(dialogueTimer);
            Console.Clear();

            Console.SetCursorPosition(37, 14);
            Console.WriteLine("Please don't bother trying to pronounce my name.");
            Thread.Sleep(dialogueTimer);
            Console.Clear();

            Console.SetCursorPosition(55, 14);
            Console.WriteLine("You are");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.SetCursorPosition(63, 14);
            Console.WriteLine("dead.");
            Console.ForegroundColor = ConsoleColor.Red;
            Thread.Sleep(dialogueTimer);
            Console.Clear();

            Console.SetCursorPosition(36, 14);
            Console.WriteLine("Things may seem a bit grim... and indeed they are.");
            Thread.Sleep(dialogueTimer);
            Console.Clear();

            Console.SetCursorPosition(35, 14);
            Console.WriteLine("But not to worry. Let's start with something simple...");
            Thread.Sleep(dialogueTimer);
            Console.Clear();

            Console.SetCursorPosition(31, 14);
            Console.WriteLine("Try to recall your life on Earth. Do you remember your name?");
            Thread.Sleep(dialogueTimer);
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.SetCursorPosition(45, 14);
            Console.Write("Enter your name here: ");
            string input = Console.ReadLine();

            //this if statement makes the player's name the default if they enter nothing
            if (input.Length > 0)
            {
                playerName = input;
            }

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;

            Console.SetCursorPosition(33, 14);
            Console.WriteLine($"Well, {playerName}, you're probably wondering why you're here...");
            Thread.Sleep(dialogueTimer);
            Console.Clear();

            Console.SetCursorPosition(39, 14);
            Console.WriteLine("""Or, perhaps, you're wondering what "here" is.""");
            Thread.Sleep(dialogueTimer);
            Console.Clear();

            Console.SetCursorPosition(46, 14);
            Console.WriteLine("This is a place of punishment.");
            Thread.Sleep(dialogueTimer);
            Console.Clear();

            Console.SetCursorPosition(24, 14);
            Console.WriteLine("You must have done something truly sinister in your past life to end up here.");
            Thread.Sleep(dialogueTimer);
            Console.Clear();

            Console.SetCursorPosition(23, 14);
            Console.WriteLine("With that being said, I would like to offer you a final chance at freedom...");
            Thread.Sleep(dialogueTimer);
            Console.Clear();

            Console.SetCursorPosition(23, 14);
            Console.WriteLine("Navigate my dungeon, and triumph against the obstacles I have put before you...");
            Thread.Sleep(dialogueTimer);
            Console.Clear();

            Console.SetCursorPosition(33, 14);
            Console.WriteLine("And I will return you to the place from which you came.");
            Thread.Sleep(dialogueTimer);
            Console.Clear();

            Console.SetCursorPosition(19, 14);
            Console.WriteLine("What's with the grimaced expression I see on your face? I have full confidence in you.");
            Thread.Sleep(dialogueTimer);
            Console.Clear();

            Console.SetCursorPosition(34, 14);
            Console.WriteLine($"But do you have full confidence in yourself... {playerName}?");
            Thread.Sleep(dialogueTimer);
            Console.Clear(); 

            Console.ForegroundColor = ConsoleColor.White;
        }

        static void PlayerHelp()
        {
            /*EXPLANATION: This method displays helpful tips to the player*/

            Console.Clear();
            Console.WriteLine("This is a placeholder for the help method. Jack says: Michael Jackson is the King of Pop");
            Console.ReadLine();
        }

        static void PlayerInventory(int position)
        {
            /*EXPLANATION: This method is for drawing and managing the player's inventory.*/

            //these items are just placeholders to demonstrate how the inventory works
            Dictionary<int, string> inventory = new Dictionary<int, string>()
            {
                [1] = "Item One",
                [2] = "Item Two",
                [3] = "Item Three",
                [4] = "Item Four",
                [5] = "Item Five"
            };

            Console.Write($" Currently Equipped [{position}]: {inventory[position]}");
        }

        static void PlayerMovement()
        {
            /*EXPLANATION: This method is a simple WASD movement system. It continuously checks for user input and changes their position accordingly
            TODO: Prevent the game from crashing if the cursor is moved outside of the window (consider the boundaries of the map)*/

            //center position while windowed is 60, 14
            int x = 60, y = 14, inventoryPosition = 1;

            char movement = 'z';

            bool isMoving = true;

            while (isMoving)
            {
                //this big block is for all of the information at the top of the screen
                Console.BackgroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($" -@-  Enter WASD to move. Enter 'e' to stop moving. Enter - or = to cycle through your inventory. Enter 'h' for help.");
                Console.BackgroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine($"X:{x:D3} Y:{y:D2} | ");
                Console.SetCursorPosition(12, 1);
                PlayerInventory(inventoryPosition);
                Console.BackgroundColor = ConsoleColor.Black;

                //Draws the map to the screen
                SpawnRoom(currentLevel);

                // Draw the player on top of the map
                Console.SetCursorPosition(x, y);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("X");
                Console.ForegroundColor = ConsoleColor.White;

                //this block of code handles user input
                Console.SetCursorPosition(0, 27);
                Console.Write("Input: ");
                string input = Console.ReadLine().ToLower();

                //this if statement prevents a crash from happening if the user inputs nothing
                if (input.Length > 0)
                {
                    movement = Convert.ToChar(input.Substring(0, 1));
                }

                switch (movement)
                {
                    case 'w':
                        if(y > 4)
                        {
                            y -= 1;
                        }
                        break;
                    case 'a':
                        if(x > 4)
                        {
                            x -= 1;
                        }
                        break;
                    case 's':
                        if(y < 18)
                        {
                            y += 1;
                        }
                        break;
                    case 'd':
                        if(x < 111)
                        {
                            x += 1;
                        }
                        break;
                    case 'e':
                        isMoving = false;
                        break;
                    case 'h':
                        PlayerHelp();
                        break;
                    case '-':
                        if (inventoryPosition > 1)
                        {
                            inventoryPosition -= 1;
                        }
                        break;
                    case '=':
                        if (inventoryPosition < 5)
                        {
                            inventoryPosition += 1;
                        }
                        break;
                    default:
                        break;
                }

                int[] positions = CheckRoom(x, y);

                x = positions[0];
                y = positions[1];

                //clear the screen to prevent trailing
                Console.Clear();
            }
        }

        static int[] CheckRoom(int x, int y)
        {
            if (currentLevel == "levelOne")
            {
                if (x == 4 && y == 8)
                {
                    currentLevel = "levelTwo";

                    return new [] { 108, 8 };
                }
                else if (x == 111 && y == 13)
                {
                    currentLevel = "levelThree";

                    return new [] { 6, 13 };
                }
            }

            if (currentLevel == "levelTwo")
            {
                if (x == 109 && y == 8)
                {
                    currentLevel = "levelOne";

                    return new [] { 5, 8 };
                }
            }

            if (currentLevel == "levelThree")
            {
                if (x == 5 && y == 13)
                {
                    currentLevel = "levelOne";

                    return new [] { 110, 13 };
                }
            }

            return new [] { x, y };
        }

        static void SpawnRoom(string nextLevel)
        {
            currentLevel = nextLevel;

            string filePath = $"Assets/{currentLevel}.txt";

            //this dictionary just gives each level a nice name
            Dictionary<string, string> RoomNames = new Dictionary<string, string>()
            {
                { "levelOne", "Main Chamber"},
                { "levelTwo", "Left Room"},
                { "levelThree", "Right Room"},
            };

            if (File.Exists(filePath))
            {
                Console.BackgroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine($" | Room: {RoomNames[currentLevel]}");
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Gray;

                string levelContents = File.ReadAllText(filePath);
                Console.WriteLine($"\n{levelContents}");
            }
            else
            {
                Console.WriteLine($"Error: Can't find the file: {filePath}");
            }

        }
    }
}
