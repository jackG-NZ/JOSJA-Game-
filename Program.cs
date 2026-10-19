using System.Diagnostics;

namespace JOSJA_Game
{
    internal class Program
    {
        static string currentLevel = "level1";

        static int x = 100, y = 10, inventoryPosition = 1;             //center position while windowed is 58, 14

        //these define the boundaries of each level. The values are adjusted in the checkRoom method
        static int[] xBoundaries = { 0, 0 }; //left x boundary, right x boundary
        static int[] yBoundaries = { 0, 0 }; //top y boundary, bottom y boundary

        static bool isInGame = true;

        static double hp = 20;
        const double RegenPerTurn = 0.5;
        const double PoisonEffectPerTurn = -2.0;
        const double HealthPotionRegen = 10.0;
        const double HealingSalveRegen = 3.5;

        static double enemyHP = 10;
        static double enemyAttackModifier = 1;

        static Dictionary<int, string> Items = new Dictionary<int, string>()
        {
            // Misc
            [1] = "Empty Handed",
            
            // Items
            [2] = "Key",
            [3] = "Crystal Shard",
            [4] = "Wooden Sword",
            [5] = "Iron Sword",
            [6] = "Diamond Sword",

            // Foods
            [7] = "Dungeon Berries",
            [8] = "Raw Meat",
            [9] = "Cooked Meat",

            // Consumables
            [10] = "Healing Salve",
            [11] = "Antidote",
            [12] = "Poison Bottle",

            // Ingredients
            [13] = "Cave Moss",
            [14] = "Spider Fang",
            [15] = "Stone",
            [16] = "Slime",

            // Story/Lore Items
            [17] = "Portal Component",
        };

        /*static Dictionary<string, int[]> MapSizes = new Dictionary<string, int[]>
        {
            { "level1X", new int[] { 4, 111 } }, //LEFT X BOUNDARY, RIGHT X BOUNDARY
            { "level1Y", new int[] { 3, 16 } }, //TOP Y BOUNDARY, BOTTOM Y BOUNDARY
        };*/

        static Dictionary<int, string> Inventory = new Dictionary<int, string>()
        {
            [1] = Items[2], //the player starts with a key, a wooden sword, and three empty inventory slots
            [2] = Items[4],
            [3] = Items[1],
            [4] = Items[1],
            [5] = Items[1],
        };

        static void Main()
        {
            GameIntro();

            while (isInGame) //will eventually add some sort of condition here
            {
                PlayerActions();
            }

            Console.Clear();

            Console.WriteLine("Thank you for playing our game!");

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

            if (MainMenu()) 
                return; // continuing a save skips the intro

            Console.SetCursorPosition(35, 14);
            Console.Write("Would you like to skip the opening dialogue? (y|n): ");
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

        static bool MainMenu()
        {
            /*EXPLANATION: This method gives the player a choice to start a new game or continue from their save file*/

            Console.SetCursorPosition(35, 13);
            Console.WriteLine("1. Continue");
            Console.SetCursorPosition(35, 14);
            Console.WriteLine("2. New Game");
            Console.SetCursorPosition(35, 16);
            Console.Write("Choose (1|2): ");

            int choice = Convert.ToInt32(Console.ReadLine());
            Console.Clear();

            if (choice == 1)
            {
                SaveAndLoad(true);
                return true;
            }
            return false;
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

            Console.SetCursorPosition(54, 14);
            Console.WriteLine("You are");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.SetCursorPosition(62, 14);
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

            Console.SetCursorPosition(30, 14);
            Console.WriteLine("With that being said, I'm in the mood for some entertainment.");
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

            Console.SetCursorPosition(33, 14);
            Console.WriteLine("Fail, and I will never let you leave. Well? Off you go!.");
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

        static void PlayerActions()
        {
            /*EXPLANATION: This method contains the main gameplay loop. It continuously checks for user input and changes their position or performs actions accordingly*/

            char action = 'z';

            int previousAction = 0; //this records the player's previous action so the enemy AI can react appropriately

            bool isFighting = false;
            bool isMoving = true;
            bool isPlayerTurn = false;

            if (currentLevel == "enemyEncounter")
            {
                isFighting = true;
            }

            while (isMoving && isInGame)
            {
                //this big block is for all of the information at the top of the screen
                if (isFighting) //different information is displayed to the user if they're fighting
                {
                    Console.BackgroundColor = ConsoleColor.DarkGray;
                    Console.Write($" -@-  Battle! ");
                }
                else
                {
                    Console.BackgroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine($"    --=@=--    |Enter WASD to move. Enter - or = to cycle through your inventory. Enter 'h' for help.|    --=@=--    ");
                    Console.BackgroundColor = ConsoleColor.DarkMagenta;
                    Console.Write($"Press 'p' to save progress. | X:{x:D3} Y:{y:D2} | ");
                    Console.SetCursorPosition(43, 1);

                    Console.Write($"Currently Equipped [{inventoryPosition}]: {Inventory[inventoryPosition]} ");
                    Console.BackgroundColor = ConsoleColor.Black;
                }

                //Draws the map to the screen
                SpawnRoom(currentLevel);

                // Draw the player on top of the map
                Console.SetCursorPosition(x, y);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("X");
                Console.ForegroundColor = ConsoleColor.White;

                //Manages encounters
                if(EncounterManagement(currentLevel, x, y) == true)
                {
                    y -= 2;
                }

                if (isFighting)
                {
                    Console.SetCursorPosition(4, 2);
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.Write($"Your HP:  {new string('|', (int)hp)}");
                    Console.SetCursorPosition(4, 3);
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.Write($"Enemy HP: {new string('|', (int)enemyHP)}");
                    Console.BackgroundColor = ConsoleColor.Black;

                    if(enemyHP > 0)
                    {
                        EnemyAIReaction(previousAction);
                    }
                    else
                    {
                        Console.SetCursorPosition(48, 2);
                        Console.Write("The enemy has died!");

                        isFighting = false;

                        currentLevel = "level1";
                    }

                    if (isPlayerTurn)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        Console.SetCursorPosition(53, 16);
                        Console.Write("YOUR TURN!");
                        Console.BackgroundColor = ConsoleColor.Black;
                    }
                    else
                    {
                        Console.BackgroundColor = ConsoleColor.DarkRed;
                        Console.SetCursorPosition(53, 16);
                        Console.Write("ENEMY TURN");
                        Console.BackgroundColor = ConsoleColor.Black;
                    }

                    //Draws the input options 
                    Console.SetCursorPosition(21, 18);
                    Console.Write("Attack");
                    Console.SetCursorPosition(20, 19);
                    Console.Write("ENTER 1");

                    Console.SetCursorPosition(44, 18);
                    Console.Write("Block");
                    Console.SetCursorPosition(43, 19);
                    Console.Write("ENTER 2");

                    Console.SetCursorPosition(68, 18);
                    Console.Write("Heal");
                    Console.SetCursorPosition(67, 19);
                    Console.Write("ENTER 3");

                    Console.SetCursorPosition(91, 18);
                    Console.Write("Flee");
                    Console.SetCursorPosition(90, 19);
                    Console.Write("ENTER 4");
                }

                //this block of code handles user input
                Console.SetCursorPosition(0, 27);
                Console.Write("Input: ");
                action = Console.ReadKey().KeyChar;
                

                //this big switch statement handles all of the player's actions. This includes movement, combat, and utility
                switch (action)
                {
                    case 'w':
                        if (y > yBoundaries[0] && !isFighting && !IsWall(x, y - 1))
                        {
                            y -= 1;
                        }
                        break;
                    case 'a':
                        if (x > xBoundaries[0] && !isFighting && !IsWall(x - 1, y))
                        {
                            x -= 1;
                        }
                        break;
                    case 's':
                        if (y < yBoundaries[1] && !isFighting && !IsWall(x, y + 1))
                        {
                            y += 1;
                        }
                        break;
                    case 'd':
                        if (x < xBoundaries[1] && !isFighting && !IsWall(x + 1, y))
                        {
                            x += 1;
                        }
                        break;
                    case 'h':
                        PlayerHelp();
                        break;
                    case '1':
                        if (isPlayerTurn)
                        {
                            PlayerAttack();
                            previousAction = 1;
                        }
                        break;
                    case '2':
                        if (isPlayerTurn)
                        {
                            PlayerBlock();
                            previousAction = 2;
                        }
                        break;
                    case '3':
                        if (isPlayerTurn)
                        {
                            PlayerHeal();
                            previousAction = 3;
                        }
                        break;
                    case '4':
                        if (isPlayerTurn)
                        {
                            PlayerFlee();
                            previousAction = 4;
                        }
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
                    case 'p':
                        SaveAndLoad(false);
                        break;
                    default:
                        break;
                }

                isPlayerTurn = !isPlayerTurn;

                //the CheckRoom method returns the x, y coordinates of the starting point for each room
                int[] positions = CheckRoom(x, y, xBoundaries, yBoundaries);

                x = positions[0];
                y = positions[1];

                //clear the screen to prevent trailing
                Console.Clear();
            }
        }

        static bool IsWall(int x, int y) //checks if the player is walking into a wall
        {
            string[] map = File.ReadAllLines($"Assets/Levels/{currentLevel}.txt");

            int row = y-2;
            int col = x;

            if (y-2 < 0 || y-2 >= map.Length) { return false; }
            if (x < 0 || x >= map[row].Length) { return false; }

            return map[row][col] == '#' || map[row][col] == '|' || map[row][col] == '/' || map[row][col] == '\\';
        }

        static int[] CheckRoom(int x, int y, int[] xBoundaries, int[] yBoundaries)
        {
            /*EXPLANATION: This method checks what room the player is in and their current position. If they reach the x,y coordinates of a doorway, the room changes
            and returns the starting x, y coordinates of the new room in an integer array. It also adjusts the movement boundary for each level*/

            switch (currentLevel)
            {
                case "level1":

                    xBoundaries[0] = 4;
                    xBoundaries[1] = 104;
                    yBoundaries[0] = 5;
                    yBoundaries[1] = 15;

                    if (x == 4 && y == 10)
                    {
                        currentLevel = "level2";

                        return new[] { 109, 12 };
                    }
                    break;
                case "level2":

                    xBoundaries[0] = 4;
                    xBoundaries[1] = 111;
                    yBoundaries[0] = 3;
                    yBoundaries[1] = 17;

                    if (x == 111 && y == 12)
                    {
                        currentLevel = "level1";

                        return new[] { 5, 10 };
                    }
                    else if (x == 4 && y == 7)
                    {
                        currentLevel = "level3";

                        return new[] { 107, 7 };
                    }
                    break;
                case "level3":

                    xBoundaries[0] = 4;
                    xBoundaries[1] = 110;
                    yBoundaries[0] = 3;
                    yBoundaries[1] = 17;

                    if (x == 109 && y == 7)
                    {
                        currentLevel = "level2";

                        return new[] { 6, 7 };
                    }
                    else if (x == 54 && y == 3)
                    {
                        currentLevel = "level5";

                        return new[] { 18, 16 };
                    }
                    else if (x == 54 && y == 17)
                    {
                        currentLevel = "level4";

                        return new[] { 54, 4 };
                    }
                    break;

                case "level4":

                    xBoundaries[0] = 4;
                    xBoundaries[1] = 109;
                    yBoundaries[0] = 3;
                    yBoundaries[1] = 17;
                    if (x == 54 && y == 03)
                    {
                        currentLevel = "level3";

                        return new[] { 54, 16 };
                    }
                    break;

                case "level5":

                    xBoundaries[0] = 4;
                    xBoundaries[1] = 50;
                    yBoundaries[0] = 3;
                    yBoundaries[1] = 17;
                    if (x == 18 && y == 17)
                    {
                        currentLevel = "level3";

                        return new[] { 54, 04 };
                    }
                    else if (x == 4 && y == 15)
                    {
                        currentLevel = "level6";

                        return new[] { 108, 15 };
                    }
                    else if (x == 18 && y == 3)
                    {
                        currentLevel = "level7";

                        return new[] { 28, 16 };
                    }
                    break;

                case "level6":

                    xBoundaries[0] = 4;
                    xBoundaries[1] = 111;
                    yBoundaries[0] = 3;
                    yBoundaries[1] = 17;
                    if (x == 110 && y == 15)
                    {
                        currentLevel = "level5";

                        return new[] { 6, 15 };
                    }
                    break;

                case "level7":

                    xBoundaries[0] = 4;
                    xBoundaries[1] = 110;
                    yBoundaries[0] = 3;
                    yBoundaries[1] = 17;
                    if (x == 28 && y == 17)
                    {
                        currentLevel = "level5";

                        return new[] { 18, 4 };
                    }
                    else if (x == 109 && y == 7)
                    {
                        currentLevel = "level8";

                        return new[] { 6, 9 };
                    }
                    else if (x == 109 && y == 7)
                    {
                        currentLevel = "level1";

                        return new[] { 6, 9 };
                    }
                    break;
                case "level8":

                    xBoundaries[0] = 4;
                    xBoundaries[1] = 86;
                    yBoundaries[0] = 3;
                    yBoundaries[1] = 17;
                    if (x == 4 && y == 9)
                    {
                        currentLevel = "level7";

                        return new[] { 107, 7 };
                    }
                    else if (x == 85 && y == 12)
                    {
                        currentLevel = "level9";

                        return new[] { 6, 12 };
                    }
                    break;
                case "level9":

                    xBoundaries[0] = 4;
                    xBoundaries[1] = 86;
                    yBoundaries[0] = 3;
                    yBoundaries[1] = 17;
                    if (x == 4 && y == 12)
                    {
                        currentLevel = "level8";

                        return new[] { 83, 12 };
                    }
                    break;

                default:
                    Console.Clear();
                    Console.WriteLine("An unexpected error has occured.");
                    break;
            }

            return new[] { x, y };
        }

        static bool EncounterManagement(string currentLevel, int x, int y)
        {
            /*EXPLANATION: This method handles all encounters with items, enemies, interactables etc*/

            char playerInteract = 'z';

            bool hasEncountered = false;
            
            switch (currentLevel) 
            {
                case "level1":
                    if (x == 104 && y == 10)
                    {
                        Console.SetCursorPosition(31, 9);
                        Console.BackgroundColor = ConsoleColor.DarkBlue;
                        Console.WriteLine("This is where you entered from. It caved in just as you walked through.");
                        Console.BackgroundColor = ConsoleColor.Black;
                    }
                    else
                    {
                        Console.SetCursorPosition(104, 10);
                        Console.Write('?');
                    }
                    if (x == 10 && y == 10)
                    {
                        Console.SetCursorPosition(23, 9);
                        Console.BackgroundColor = ConsoleColor.DarkBlue;
                        Console.WriteLine("Cauldron - press i to interact.");
                        Console.ReadLine();
                        Brewing();
                    }
                    else
                    {
                        Console.SetCursorPosition(10, 10);
                        Console.Write('?');
                    }
                    break;
                case "level2":
                    int chestItem = 4;
                    if (x == 60 && y == 14 && chestItem == 4) //chest
                    {
                        Console.SetCursorPosition(31, 9);
                        Console.BackgroundColor = ConsoleColor.DarkBlue;
                        Console.WriteLine("The chest opens! Enter 'i' to interact.");
                        Console.BackgroundColor = ConsoleColor.Black;
                        Interaction(chestItem);
                    }
                    else
                    {
                        Console.SetCursorPosition(60, 14);
                        Console.Write('?');
                    }
                    break;
                case "level9": //RIGHT ROOM CHEST INTERACTION
                    if (x == 54 && y == 13 && Inventory.ContainsValue(Items[2])) //will only execute if the coordinates are correct AND if the player has the key (Items[2])
                    {
                        Console.SetCursorPosition(31, 9);
                        Console.BackgroundColor = ConsoleColor.DarkBlue;
                        Console.WriteLine("Your Key fits the lock! Enter 'i' to interact.");
                        Console.BackgroundColor = ConsoleColor.Black;

                        Interaction(17); //17 is the key of the portal component in the item dictionary

                        hasEncountered = true;
                    }
                    break;
                
                case "level4": //BOTTOM ROOM PORTAL INTERACTION/GAME END
                    if(x == 46 && y == 10 && Inventory.ContainsValue(Items[17]))
                    {
                        Console.SetCursorPosition(23, 9);
                        Console.BackgroundColor = ConsoleColor.DarkBlue;
                        Console.WriteLine("Your Portal Component fits! Enter 'i' to interact.");

                        GameEnd(false);
                    }
                    break;
            }

            return hasEncountered;
        }

        static void Interaction(int interactionID)
        {
            char playerInteract = 'z', overWrite = 'y';

            int inventorySlot = -1;

            Console.SetCursorPosition(31, 10);
            Console.Write("Input: ");
            playerInteract = Console.ReadKey().KeyChar;

            if (playerInteract == 'i')
            {
                do
                {
                    Console.Clear();
                    Console.SetCursorPosition(51, 11);
                    Console.BackgroundColor = ConsoleColor.DarkBlue;
                    Console.Write("==== INVENTORY ====");
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.SetCursorPosition(44, 13);
                    Console.WriteLine($"You picked up a {Items[interactionID]}!");
                    Console.SetCursorPosition(44, 14);
                    Console.Write("Choose an inventory slot (1 - 5): ");

                    try
                    {
                        inventorySlot = Convert.ToInt32(Console.ReadLine());
                    }
                    catch
                    {
                        Console.SetCursorPosition(43, 15);
                        Console.Write("          ! Input Error !         ");
                        Thread.Sleep(3000);
                    }

                } while (inventorySlot > 5 || inventorySlot < 0);

                if (Inventory[inventorySlot] != Items[1])
                {
                    Console.Clear();
                    Console.SetCursorPosition(25, 13);
                    Console.Write("Warning! This slot is already taken. Do you wish to overwrite its contents?");
                    Console.SetCursorPosition(25, 14);
                    Console.Write($"Slot {inventorySlot} is currently occupied by the item: {Inventory[inventorySlot]}.");
                    Console.SetCursorPosition(25, 15);
                    Console.Write($"Press [ENTER] to overwrite. Enter 'n' to cancel: ");

                    playerInteract = Console.ReadKey().KeyChar;

                }

                if (overWrite == 'y')
                {
                    Console.Clear();
                    Inventory[inventorySlot] = Items[interactionID];
                    Console.SetCursorPosition(38, 11);
                    Console.BackgroundColor = ConsoleColor.DarkBlue;
                    Console.Write($"Slot overwritten by object {Items[interactionID]}!");
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.SetCursorPosition(38, 13);
                    Console.Write("Press [ENTER] to return to game.");
                }
                else
                {
                    Console.Clear();
                    Console.SetCursorPosition(28, 13);
                    Console.Write($"This slot will not be overwritten. Press [ENTER] to return to game.");
                }
            }
        }

        static void SpawnRoom(string nextLevel)
        {
            /*EXPLANATION: This method handles the printing of each room to the screen*/

            currentLevel = nextLevel;

            string filePath = $"Assets/Levels/{currentLevel}.txt";

            //this dictionary just gives each level a nice name
            Dictionary<string, string> RoomNames = new Dictionary<string, string>()
            {
                { "level1", "Entrance Room"},
                { "level2", "First Chamber"},
                { "level3", "Right Room"},
                { "level4", "Left Storage Room" },
                { "level5", "Cell" },
                { "level6", "Cell" },
                { "level7", "Cell" },
                { "level8", "Cell" },
                { "level9", "Cell" },
                { "enemyEncounter", "BATTLE" },
            };

            if (File.Exists(filePath))
            {
                Console.BackgroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine($" | Room: {RoomNames[currentLevel]}");
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Gray;

                string levelContents = File.ReadAllText(filePath);
                Console.WriteLine($"{levelContents}");
            }
            else
            {
                Console.WriteLine($"Error: Can't find the file: {filePath}");
            }

        }

        static void SaveAndLoad(bool load)
        {
            const string saveFolder = "PlayerData";
            string savePath = Path.Combine(saveFolder, "save.txt");

            if (load)
            {
                if (!File.Exists(savePath)) return;

                StreamReader reader = new StreamReader(savePath);               //StreamReader is a tool used to read a file line by line
                currentLevel = reader.ReadLine().Replace("Level = ", "");       //Uses .Replace to get rid of the human readable formatting and only keeps the saved info
                x = Convert.ToInt32(reader.ReadLine().Replace("X = ", ""));     
                y = Convert.ToInt32(reader.ReadLine().Replace("Y = ", ""));
                hp = Convert.ToDouble(reader.ReadLine().Replace("Health = ", ""));
                inventoryPosition = Convert.ToInt32(reader.ReadLine().Replace("Inventory = ", ""));
                // Load each inventory slot
                for (int i = 1; i <= 5; i++)
                {
                    string line = reader.ReadLine();
                    Inventory[i] = line.Replace("Slot " + i + " = ", "");
                }
                reader.Close();     //close reader
            }
            else
            {
                Console.WriteLine("Are you sure you want to overwrite save file? [y/n]");
                string temp = Console.ReadLine().ToLower();

                if (temp == "y")
                {
                    Directory.CreateDirectory(saveFolder);

                    string saveContents =
                        "Level = " + currentLevel + "\n" +
                        "X = " + x + "\n" +
                        "Y = " + y + "\n" +
                        "Health = " + hp + "\n" +
                        "Inventory = " + inventoryPosition + "\n";
                        // Save each slot
                        foreach (var item in Inventory)
                        {
                            saveContents += "Slot " + item.Key + " = " + item.Value + "\n";
                        }

                    File.WriteAllText(savePath, saveContents);
                    Console.WriteLine("Saved!");
                }
                else
                {
                    Console.WriteLine("Aborted!");
                }
                Console.WriteLine("Press [Enter] to continue (or close the window to exit)");
                Console.ReadLine();
            }
        }

        static void PlayerAttack()
        {
            /*EXPLANATION: This is the method for the player's attack*/

            Random rand = new Random();
            int attackPower = rand.Next(1, 5);

            if(enemyHP - attackPower < 0)
            {
                enemyHP = 0;
            }
            else
            {
                enemyHP -= attackPower;
            }
        }

        static void PlayerBlock()
        {
            /*EXPLANATION: This is the method for the block utility*/

            enemyAttackModifier = 0;
        }

        static void PlayerHeal()
        {
            /*EXPLANATION: This is the method for the heal utility*/
        }

        static void PlayerFlee()
        {
            /*EXPLANATION: This is the method for the flee utility*/
        }

        static void EnemyAIReaction(int previousAction)
        {
            switch (previousAction)
            {
                case 0:
                    Console.SetCursorPosition(46, 2);
                    Console.Write("No player action detected.");

                    EnemyAIAttack();

                    break;
                case 1:
                    Console.SetCursorPosition(41, 2);
                    Console.Write("The player has attacked the enemy!");

                    EnemyAIAttack();

                    break;
                case 2:
                    Console.SetCursorPosition(39, 2);
                    Console.Write("The player will block the incoming attack.");

                    EnemyAIAttack();

                    break;
                case 3:
                    Console.SetCursorPosition(44, 2);
                    Console.Write("The player has healed themselves.");

                    EnemyAIAttack();

                    break;
                case 4:
                    Console.SetCursorPosition(46, 2);
                    Console.Write("The player is running away!");
                    break;
                default:
                    Console.SetCursorPosition(45, 2);
                    Console.Write("An unexpected error has occurred.");
                    break;
            }

            enemyAttackModifier = 1;
        }

        static void EnemyAIAttack()
        {
            Random rand = new Random();
            double attackPower = rand.Next(1, 3);

            if(hp - attackPower < 0)
            {
                hp = 0;
            }
            else
            {
                hp -= attackPower * enemyAttackModifier;
            }

            HP(hp);
        }

        static void HP(double hp)
        {
            if (hp < 1)
            {
                GameEnd(true);
            }
        }

        static void Brewing()
        {
            Console.Clear();
            string filePath = $"Assets/UIs/Cauldron.txt";
            if (File.Exists(filePath))
            {
                Console.BackgroundColor = ConsoleColor.DarkMagenta;

                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Gray;

                string CauldronUI = File.ReadAllText(filePath);
                Console.WriteLine($"{CauldronUI}");
            }
            else
            {
                Console.WriteLine($"Error: Can't find the file: {filePath}");
            }
        }

        static void GameEnd(bool dead)
        {
            if (dead)
            {
                char Restore = 'n';
                Console.Clear();
                Console.SetCursorPosition(23, 10);
                Console.WriteLine("You Died.");
                Thread.Sleep(3000);
                Console.Clear();
                Console.WriteLine("Would you like re-spawn from last save? [y/n]");
                string input = (Console.ReadLine().ToLower());
                if (input.Length > 0)
                {
                    Restore = Convert.ToChar(input.Substring(0, 1));
                }

                if (Restore == 'y')
                {
                    SaveAndLoad(true);
                }

            }
            else
            {
                char playerInteract = 'z';

                Console.BackgroundColor = ConsoleColor.Black;
                Console.SetCursorPosition(23, 10);
                Console.Write(": ");

                string input = Console.ReadLine().ToLower();
                if (input.Length > 0)
                {
                    playerInteract = Convert.ToChar(input.Substring(0, 1));
                }

                if (playerInteract == 'i')
                {
                    Console.Clear();
                    Console.WriteLine("This is the end of the game. Press [ENTER] to proceed.");

                    isInGame = false;
                }
            }
        }
    }
}
