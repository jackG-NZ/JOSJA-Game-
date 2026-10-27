using Microsoft.VisualBasic;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Reflection;

namespace JOSJA_Game
{
    internal class Program
    {
        static string currentLevel = "level1";
        static string enemyName = "placeholder"; //this changes depending on the enemy encounter
        static string previousLevel = "level1"; //this saves the previous level so the game knows where to return the player after combat

        static int x = 100, y = 10, inventoryPosition = 1;             //center position while windowed is 58, 14

        //these define the boundaries of each level. The values are adjusted in the checkRoom method
        static int[] xBoundaries = { 0, 0 }; //left x boundary, right x boundary
        static int[] yBoundaries = { 0, 0 }; //top y boundary, bottom y boundary

        static bool isInGame = true;
        static bool jasonEnemyDefeated = false; //this is kinda dumb but it stops an endless loop from happening
        static bool slimyJackEnemyDefeated = false; //this is kinda dumb but it stops an endless loop from happening

        static double hp = 20;
        const double RegenPerTurn = 0.5;
        const double PoisonEffectPerTurn = -2.0;
        const double HealthPotionRegen = 10.0;
        const double HealingSalveRegen = 3.5;

        static double enemyHP = 0;
        static double enemyAttackModifier = 1;

        static Dictionary<int, string> Items = new Dictionary<int, string>()
        {
            // Misc
            [1] = "Nothing",

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
            [10] = "Blind Cave Fish",

            // Consumables
            [11] = "Health Potion",
            [12] = "Antidote",
            [13] = "Poison Potion",
            [14] = "Strength Potion",
            [15] = "Mana Potion",
            [16] = "Stamina Potion",

            // Ingredients
            [17] = "Cave Moss",
            [18] = "Spider Fang",
            [19] = "Stone",
            [20] = "Slime",
            [21] = "Iron Shard",
            [22] = "Feather",
            [23] = "Bone",
            [24] = "Cryptcrawler Root",
            [25] = "Dungeon Bug",

            // Story/Lore Items
            [26] = "Portal Component",
            [27] = "WD-40"

        };

        /*static Dictionary<string, int[]> MapSizes = new Dictionary<string, int[]>
        {
            { "level1X", new int[] { 4, 111 } }, //LEFT X BOUNDARY, RIGHT X BOUNDARY
            { "level1Y", new int[] { 3, 16 } }, //TOP Y BOUNDARY, BOTTOM Y BOUNDARY
        };*/

        static Dictionary<int, (string ItemName, int Quantity)> InvisibleInventory = new Dictionary<int, (string, int)>()
        {
            [1] = (Items[1], 0), // Starts as "Nothing" with 0 quantity
            [2] = (Items[1], 0),
            [3] = (Items[1], 0),
            [4] = (Items[1], 0),
            [5] = (Items[1], 0),
            [6] = (Items[1], 0),
            [7] = (Items[1], 0),
            [8] = (Items[1], 0),
            [9] = (Items[1], 0),
            [10] = (Items[1], 0),
            [11] = (Items[1], 0),
            [12] = (Items[1], 0),
        };

        static Dictionary<int, string> Inventory = new Dictionary<int, string>()
        {
            [1] = Items[1], //all the needed items are now in the game in chests
            [2] = Items[1], 
            [3] = Items[1],
            [4] = Items[1],
            [5] = Items[1],
        };

        static Dictionary<int, int> Chests = new Dictionary<int, int>()
        {
            [1] = 4,
            [2] = 10,
            [3] = 21,
            [4] = 6,
            [5] = 2,
            [10] = 26,

        };

        static Dictionary<string, int[]> Enemies = new Dictionary<string, int[]>()//[Name] = [Health, Item]
        {
            ["George"] = [10, 23],
            ["Jason"] = [10, 23],
            ["Slimy Jack"] = [10, 20],

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

            int choice = 2;

            char skipDialogue = 'n'; //set this to n, so if the user enters nothing it assumes they don't want to skip the intro dialogue

            string filePath = $"Assets/UIs/MainMenu.txt";
            if (File.Exists(filePath))
            {
                string MainMenuUI = File.ReadAllText(filePath);
                Console.WriteLine($"{MainMenuUI}");
            }
            else
            {
                Console.WriteLine($"Error: Can't find the file: {filePath}");
            }

            string input1 = Console.ReadLine();

            //this if statement prevents a crash from happening if the user inputs nothing
            if (input1.Length > 0)
            {
                choice = input1[0];
            }

            Console.Clear();
            if (choice == '1')
            {
                SaveAndLoad(true);
            }
            else
            {
                Console.SetCursorPosition(35, 14);
                Console.Write("Would you like to skip the opening dialogue? (y|n): ");
                string input2 = Console.ReadLine().ToLower();

                //this if statement prevents a crash from happening if the user inputs nothing
                if (input2.Length > 0)
                {
                    skipDialogue = Convert.ToChar(input2.Substring(0, 1));
                }

                Console.Clear();

                if (skipDialogue == 'n')
                {
                    IntroDialogue(); //COMMENT OUT THIS METHOD TO SKIP THE INTRO DIALOGUE
                }
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
            Console.WriteLine("""      
                Explore the dungeon, collect items, fight enemies and escape!

                - Interact with objects by placing your 'X' cursor over "?" symbols.
                - Travel between rooms by placing your 'X' cursor over "< > ^ v" symbols.
                - Essential controls are always displayed at the top of the screen.
                """);

            Console.Write("\nYou: ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("X");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("\n\nPress [ENTER] to proceed.");

            Console.SetCursorPosition(0, 28);
            Console.WriteLine("Made by Jayden, Oliver, Seth, and Jack.");

            Console.ReadLine();
        }

        static void PlayerActions()
        {
            /*EXPLANATION: This method contains the main gameplay loop. It continuously checks for user input and changes their position or performs actions accordingly*/

            char action = 'z';

            int previousAction = 0; //this records the player's previous action so the enemy AI can react appropriately

            bool isMoving = true;
            bool isPlayerTurn = false;
            bool isFighting = false;
            bool isInteracting = false;

            while (isMoving && isInGame)
            {
                if (currentLevel == "enemyEncounter")
                {
                    isFighting = true;

                }

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
                EncounterManagement(currentLevel, x, y);

                if (isFighting)
                {
                    //I moved all the combat stuff to a single method for better readability
                    isFighting = CombatLoop(previousAction, isFighting, isPlayerTurn, Enemies);

                    previousAction = 0;
                }

                //this block of code handles user input
                if (!isFighting)
                {
                    Console.SetCursorPosition(0, 27);
                    action = Console.ReadKey().KeyChar;
                    isInteracting = false;
                }
                else
                {
                    //the ReadKey movement is great but it bugs out the combat system so I've added this here
                    Console.SetCursorPosition(0, 27);
                    string input = Console.ReadLine().ToLower();
                    if(input.Length > 0)
                    {
                        action = Convert.ToChar(input.Substring(0, 1));
                    }
                }

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

                    case 'e':
                        if (currentLevel == "level11" && x == 10 && y == 10)
                        {
                            Brewing();
                            Console.ReadLine();
                        }
                        else if (currentLevel == "level2" && x == 60 && y == 14)
                        {
                            Chests[1] = Interaction(Chests[1]);
                        }
                        else if (currentLevel == "level4" && x == 105 && y == 15)
                        {
                            Chests[2] = Interaction(Chests[2]);
                        }
                        else if (currentLevel == "level4" && x == 46 && y == 10 && Inventory.ContainsValue(Items[17]))
                        {
                            GameEnd(false);
                        }
                        else if (currentLevel == "level5" && x == 46 && y == 6)
                        {
                            Chests[3] = Interaction(Chests[3]);
                        }
                        else if (currentLevel == "level6" && x == 7 && y == 11)
                        {
                            Chests[5] = Interaction(Chests[5]);
                        }
                        else if (currentLevel == "level7" && ((x == 40 && y == 6) || (x == 41 && y == 6)))
                        {
                            Enemies["Jason"][1] = Interaction(Enemies["Jason"][1]);
                        }
                        else if (currentLevel == "level9" && x == 54 && y == 13 && Inventory.ContainsValue(Items[2]))
                        {
                            Chests[10] = Interaction(Chests[10]);
                        }
                        else if (currentLevel == "level12" && ((x == 18 && y == 10) || (x == 19 && y == 10)))
                        {
                            Enemies["Slimy Jack"][1] = Interaction(Enemies["Slimy Jack"][1]);
                        }
                        else if (currentLevel == "level12" && x == 54 && y == 10)
                        {
                            Chests[4] = Interaction(Chests[4]);
                        }
                        break;

                    case 'h':
                        PlayerHelp();
                        break;
                    case '1':
                        if (isPlayerTurn)
                        {
                            PlayerAttack(CurrentWeapon(Inventory));
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
                        if (!isFighting)
                        {
                            SaveAndLoad(false);
                        }
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

        static bool CombatLoop(int previousAction, bool isFighting, bool isPlayerTurn, Dictionary<string, int[]> Enemy)
        {
            /*EXPLANATION: This method handles pretty much all of the crucial combat functionality.*/

            if (isFighting)
            {
                Console.SetCursorPosition(4, 2);
                Console.BackgroundColor = ConsoleColor.DarkGray;
                Console.Write($"Your HP:  ");
                Console.SetCursorPosition(14, 2);
                Console.BackgroundColor = ConsoleColor.Red;
                Console.Write($"{new string('|', (int)hp)}");

                Console.SetCursorPosition(4, 3);
                Console.BackgroundColor = ConsoleColor.DarkGray;
                Console.Write($"Enemy HP: ");
                Console.SetCursorPosition(14, 3);
                Console.BackgroundColor = ConsoleColor.Red;
                Console.Write($"{new string('|', (int)enemyHP)}");
                Console.BackgroundColor = ConsoleColor.Black;

                Console.SetCursorPosition(4, 4);
                Console.Write($"Enemy: Angry Skeleton {enemyName}"); //eventually change this when we have multiple enemy types?

                DisplayEnemy(enemyName);
                DisplayWeapon(CurrentWeapon(Inventory));

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

                if (enemyHP > 0)
                {
                    EnemyAIReaction(previousAction);

                    //Draws the input options 
                    if (isPlayerTurn)
                    {
                        Console.SetCursorPosition(32, 18);
                        Console.Write("Attack");
                        Console.SetCursorPosition(32, 19);
                        Console.Write("ENTER 1");

                        Console.SetCursorPosition(56, 18);
                        Console.Write("Block");
                        Console.SetCursorPosition(55, 19);
                        Console.Write("ENTER 2");

                        Console.SetCursorPosition(79, 18);
                        Console.Write("Heal");
                        Console.SetCursorPosition(78, 19);
                        Console.Write("ENTER 3");
                    }
                    else
                    {
                        Console.SetCursorPosition(31, 18);
                        Console.Write("WHEN READY");
                        Console.SetCursorPosition(30, 19);
                        Console.Write("PRESS ENTER");
                    }
                }
                else
                {
                    Console.SetCursorPosition(48, 2);
                    Console.Write("The enemy has died!");

                    Console.Beep(500, 175);
                    Console.Beep(400, 175);
                    Console.Beep(450, 175);
                    Console.Beep(500, 175);
                    Console.Beep(600, 500);

                    Console.Clear();

                    isFighting = false;

                    currentLevel = previousLevel;

                    Console.SetCursorPosition(39, 15);
                    Console.Write("Congratulations! Press [ENTER] to proceed.");

                    if(previousLevel == "level7")
                    {
                        jasonEnemyDefeated = true;
                    }
                    if (previousLevel == "level12")
                    {
                        slimyJackEnemyDefeated = true;
                    }
                }
            }

            return isFighting;
        }

        static string CurrentWeapon(Dictionary<int, string> Inventory)
        {
            /*EXPLANATION:This finds and returns the player's best weapon when they enter combat*/

            if (Inventory.ContainsValue(Items[6])) //Diamond Sword
            {
                return Items[6];
            }
            else if (Inventory.ContainsValue(Items[5])) //Iron Sword
            {
                return Items[5];
            }
            else if (Inventory.ContainsValue(Items[4])) //Wooden Sword
            {
                return Items[4];
            }

            return "Empty Handed";
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

                        return new[] { 103, 15 };
                    }
                    else if (x == 18 && y == 3)
                    {
                        currentLevel = "level7";

                        return new[] { 29, 16 };
                    }
                    break;

                case "level6":

                    xBoundaries[0] = 4;
                    xBoundaries[1] = 111;
                    yBoundaries[0] = 3;
                    yBoundaries[1] = 17;
                    if (x == 105 && y == 15)
                    {
                        currentLevel = "level5";

                        return new[] { 6, 15 };
                    }
                    break;

                case "level7":

                    xBoundaries[0] = 4;
                    xBoundaries[1] = 107;
                    yBoundaries[0] = 3;
                    yBoundaries[1] = 17;
                    if (x == 29 && y == 17)
                    {
                        currentLevel = "level5";

                        return new[] { 18, 4 };
                    }
                    else if (x == 104 && y == 7)
                    {
                        currentLevel = "level8";

                        return new[] { 6, 9 };
                    }
                    else if (x == 109 && y == 7)
                    {
                        currentLevel = "level1";

                        return new[] { 6, 9 };
                    }
                    else if (x >= 40 && !jasonEnemyDefeated) // if you walk past Jason and if he hasn't been defeated
                    {
                        EnemyEncounter("Jason", Enemies["Jason"]);
                        previousLevel = "level7";
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

                        return new[] { 102, 7 };
                    }
                    else if (x == 85 && y == 12)
                    {
                        currentLevel = "level9";

                        return new[] { 6, 12 };
                    }
                    else if (x == 48 && y == 17)
                    {
                        currentLevel = "level10";

                        return new[] { 48, 5 };
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

                case "level10":

                    xBoundaries[0] = 4;
                    xBoundaries[1] = 96;
                    yBoundaries[0] = 3;
                    yBoundaries[1] = 17;
                    if (x == 48 && y == 4)
                    {
                        currentLevel = "level8";

                        return new[] { 48, 16 };
                    }
                    else if (x == 4 && y == 10)
                    {
                        currentLevel = "level11";

                        return new[] { 38, 9 };
                    }
                    else if (x == 94 && y == 12)
                    {
                        currentLevel = "level12";

                        return new[] { 4, 10 };
                    }
                    break;
                case "level11":

                    xBoundaries[0] = 4;
                    xBoundaries[1] = 86;
                    yBoundaries[0] = 3;
                    yBoundaries[1] = 17;
                    if (x == 40 && y == 9)
                    {
                        currentLevel = "level10";

                        return new[] { 6, 10 };
                    }
                    break;
                case "level12":

                    xBoundaries[0] = 1;
                    xBoundaries[1] = 86;
                    yBoundaries[0] = 3;
                    yBoundaries[1] = 17;
                    if (x == 2 && y == 10)
                    {
                        currentLevel = "level10";

                        return new[] { 92, 12 };
                    }
                    else if (x >= 18 && !slimyJackEnemyDefeated) // if you walk past Slimy Jack and if he hasn't been defeated
                    {
                        EnemyEncounter("Slimy Jack", Enemies["Slimy Jack"]);
                        previousLevel = "level12";
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
            Console.OutputEncoding = System.Text.Encoding.UTF8;
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
                    break;
                case "level2":
                    if (x == 60 && y == 14) //chest
                    {
                        Console.SetCursorPosition(31, 9);
                        Console.BackgroundColor = ConsoleColor.DarkBlue;
                        Console.WriteLine("There's a chest, it looks like it's unlocked.");
                        Console.SetCursorPosition(31, 10);
                        Console.WriteLine($"Press E to open it.");
                        Console.BackgroundColor = ConsoleColor.Black;
                    }
                    else
                    {
                        Console.SetCursorPosition(60, 14);
                        Console.Write('?');
                    }
                    break;
                case "level4":
                    if (x == 105 && y == 15) //chest
                    {
                        Console.SetCursorPosition(31, 9);
                        Console.BackgroundColor = ConsoleColor.DarkBlue;
                        Console.WriteLine("There's a chest, it looks like it's unlocked.");
                        Console.SetCursorPosition(31, 10);
                        Console.WriteLine($"Press E to open it.");
                        Console.BackgroundColor = ConsoleColor.Black;


                    }
                    else
                    {
                        Console.SetCursorPosition(105, 15);
                        Console.Write('?');
                    }
                    if (x == 46 && y == 10) //Portal and end game
                    {
                        Console.SetCursorPosition(23, 9);
                        Console.BackgroundColor = ConsoleColor.DarkBlue;
                        Console.WriteLine("It looks like a broken portal, maybe if its fixed you could use it to get out of this place...");
                        Console.BackgroundColor = ConsoleColor.Black;
                    }
                    else if (x == 46 && y == 10 && Inventory.ContainsValue(Items[17])) //Portal and end game
                    {
                        Console.SetCursorPosition(23, 9);
                        Console.BackgroundColor = ConsoleColor.DarkBlue;
                        Console.WriteLine("Your Portal Component fits! Enter 'i' to interact.");
                        Console.BackgroundColor = ConsoleColor.Black;

                        GameEnd(false);
                    }
                    else
                    {
                        Console.SetCursorPosition(46, 10);
                        Console.Write('?');
                    }
                    break;
                case "level5":
                    if (x == 46 && y == 6)
                    {
                        Console.SetCursorPosition(31, 9);
                        Console.BackgroundColor = ConsoleColor.DarkBlue;
                        Console.WriteLine("There's a chest, it looks like it's unlocked.");
                        Console.SetCursorPosition(31, 10);
                        Console.WriteLine($"Press E to open it.");
                        Console.BackgroundColor = ConsoleColor.Black;
                    }
                    else
                    {
                        Console.SetCursorPosition(46, 6);
                        Console.Write('?');
                    }
                    break;
                case "level6":
                    if (x == 7 && y == 11)
                    {
                        Console.SetCursorPosition(31, 9);
                        Console.BackgroundColor = ConsoleColor.DarkBlue;
                        Console.WriteLine("There's a chest, it looks like it's unlocked.");
                        Console.SetCursorPosition(31, 10);
                        Console.WriteLine($"Press E to open it.");
                        Console.BackgroundColor = ConsoleColor.Black;
                    }
                    else
                    {
                        Console.SetCursorPosition(7, 11);
                        Console.Write('?');
                    }
                    break;
                case "level7":
                    if ((x == 40 &&  y == 6) || (x == 41 && y == 6))
                    {
                        Console.SetCursorPosition(31, 9);
                        Console.BackgroundColor = ConsoleColor.DarkBlue;
                        Console.WriteLine("There's a dead skeleton, it might have dropped something. Press e");
                        Console.BackgroundColor = ConsoleColor.Black;
                    }
                    else
                    {
                        Console.SetCursorPosition(40, 6);
                        Console.Write("☠︎");
                    }
                    break;
                case "level9": //RIGHT ROOM CHEST INTERACTION
                    if (x == 54 && y == 13 && Inventory.ContainsValue(Items[2]))
                    {
                        Console.SetCursorPosition(31, 9);
                        Console.BackgroundColor = ConsoleColor.DarkBlue;
                        Console.WriteLine("There's a chest, it looks like it's locked but you have the key!");
                        Console.SetCursorPosition(31, 10);
                        Console.WriteLine($"Press E to open it.");
                        Console.BackgroundColor = ConsoleColor.Black;
                    }
                    else if (x == 54 && y == 13)
                    {
                        Console.SetCursorPosition(31, 9);
                        Console.BackgroundColor = ConsoleColor.DarkBlue;
                        Console.WriteLine("There's a chest, it looks like it's locked, you should try find the key.");
                        Console.BackgroundColor = ConsoleColor.Black;
                    }
                    else
                    {
                        Console.SetCursorPosition(54, 13);
                        Console.Write('?');
                    }
                    break;
                case "level11":
                    if (x == 10 && y == 10)
                    {
                        Console.SetCursorPosition(23, 9);
                        Console.BackgroundColor = ConsoleColor.DarkBlue;
                        Console.WriteLine("Cauldron - press E to interact.");
                        Console.BackgroundColor = ConsoleColor.Black;
                    }
                    else
                    {
                        Console.SetCursorPosition(10, 10);
                        Console.Write('?');
                    }
                    break;
                case "level12":
                    if ((x == 18 && y == 10) || (x == 19 && y == 10))
                    {
                        Console.SetCursorPosition(31, 9);
                        Console.BackgroundColor = ConsoleColor.DarkBlue;
                        Console.WriteLine("There's a dead skeleton, it might have dropped something. Press e");
                        Console.BackgroundColor = ConsoleColor.Black;
                    }
                    else
                    {
                        Console.SetCursorPosition(18, 10);
                        Console.Write("☠︎");
                    }
                    if (x == 54 && y == 10)
                    {
                        Console.SetCursorPosition(31, 9);
                        Console.BackgroundColor = ConsoleColor.DarkBlue;
                        Console.WriteLine("There's a chest, it looks like it's unlocked.");
                        Console.SetCursorPosition(31, 10);
                        Console.WriteLine($"Press E to open it.");
                        Console.BackgroundColor = ConsoleColor.Black;
                    }
                    else
                    {
                        Console.SetCursorPosition(54, 10);
                        Console.Write('?');
                    }
                    break;

            }

            return hasEncountered;
        }

        static int Interaction(int interactionID)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            int inventorySlot = -1;
            int newItem = interactionID;

            Console.Clear();
            Console.BackgroundColor = ConsoleColor.Black;
            WriteLineCenter("==== INVENTORY ====", 7);
            WriteLineCenter($"Slot 1: {Inventory[1]}     Slot 2: {Inventory[2]}     Slot 3: {Inventory[3]}     Slot 4: {Inventory[4]}     Slot 5: {Inventory[5]}", 9);
            WriteLineCenter($"You got a {Items[interactionID]}!", 13);
            WriteLineCenter("Choose an inventory slot (1 - 5): ", 15);

            char key = Console.ReadKey().KeyChar;
            int.TryParse(key.ToString(), out inventorySlot);

            if (inventorySlot <= 5 && inventorySlot >= 1)
            {
                string oldItem = Inventory[inventorySlot];

                Inventory[inventorySlot] = Items[interactionID];
                newItem = Items.First(i => i.Value == oldItem).Key; //this gives the key of the item that was replaced in the inventory
                Console.WriteLine($"You swapped {oldItem} for {Items[interactionID]}");
            }
            return newItem;

            //jack old code
            //if (Inventory[inventorySlot] != Items[1])
            //{
            //    Console.Clear();
            //    Console.SetCursorPosition(25, 13);
            //    Console.Write("Warning! This slot is already taken. Do you wish to overwrite its contents?");
            //    Console.SetCursorPosition(25, 14);
            //    Console.Write($"Slot {inventorySlot} is currently occupied by the item: {Inventory[inventorySlot]}.");
            //    Console.SetCursorPosition(25, 15);
            //    Console.Write($"Press [ENTER] to overwrite. Enter 'n' to cancel: ");

            //    playerInteract = Console.ReadKey().KeyChar;

            //}

            //if (overWrite == 'y')
            //{
            //    Console.Clear();
            //    Inventory[inventorySlot] = Items[interactionID];
            //    Console.SetCursorPosition(38, 11);
            //    Console.BackgroundColor = ConsoleColor.DarkBlue;
            //    Console.Write($"Slot overwritten by object {Items[interactionID]}!");
            //    Console.BackgroundColor = ConsoleColor.Black;
            //    Console.SetCursorPosition(38, 13);
            //    Console.Write("Press [ENTER] to return to game.");
            //}
            //else
            //{
            //    Console.Clear();
            //    Console.SetCursorPosition(28, 13);
            //    Console.Write($"This slot will not be overwritten. Press [ENTER] to return to game.");
            //}
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
                { "level5", "level5" },
                { "level6", "level6" },
                { "level7", "level7" },
                { "level8", "level8" },
                { "level9", "level9" },
                { "level10", "level10" },
                { "level11", "level11" },
                { "level12", "level12" },
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

        static void EnemyEncounter(string name, int[] enemy)
        {
            /*EXPLANATION: This manages the enemy encounters*/

            currentLevel = "enemyEncounter";

            enemyHP = enemy[0];
            enemyName = name;
        }

        static void DisplayEnemy(string enemyName)
        {
            /*EXPLANATION: This displays the enemy sprite*/
            string filePath = $"Assets/Enemies/Skeleton.txt";
            if (enemyName == "Slimy Jack")
            {
                filePath = $"Assets/Enemies/SlimyJack.txt";
            }

            if (File.Exists(filePath))
            {
                string enemyContents = File.ReadAllText(filePath);
                Console.Write($"{enemyContents}");
            }
            else
            {
                Console.WriteLine($"Error: Can't find the file: {filePath}");
            }
        }

        static void DisplayWeapon(string currentWeapon)
        {
            /*EXPLANATION: This displays the weapon sprite*/

            Console.SetCursorPosition(52, 22);
            Console.WriteLine(" ___________ ");
            Console.SetCursorPosition(52, 23);
            Console.WriteLine("|           |");
            Console.SetCursorPosition(52, 24);
            Console.WriteLine("|           |");
            Console.SetCursorPosition(52, 25);
            Console.WriteLine("|___________|");

            Console.SetCursorPosition(55, 24);

            if (Inventory.ContainsValue(Items[6]) || Inventory.ContainsValue(Items[5]) || Inventory.ContainsValue(Items[4]))
            {
                StreamReader sr = new StreamReader(@"Assets/Items/Sword.txt");
                string weapon = sr.ReadToEnd();
                Console.WriteLine(weapon);
                sr.Close();
            }

            Console.SetCursorPosition(58, 24);
            Console.Write(@"X");

            Console.SetCursorPosition(43, 27);
            Console.WriteLine($"Currently equipped: {currentWeapon}");
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

                string jasonTemp = reader.ReadLine().Replace("Jason Enemy Dead = ", "");
                jasonEnemyDefeated = Convert.ToBoolean(jasonTemp);
                string slimyJackTemp = reader.ReadLine().Replace("Slimy Jack Enemy Dead = ", "");
                slimyJackEnemyDefeated = Convert.ToBoolean(slimyJackTemp);

                reader.ReadLine();  // skip the blank line
                reader.ReadLine();  // skip "--- Inventory ---"

                inventoryPosition = Convert.ToInt32(reader.ReadLine().Replace("Inventory = ", ""));
                // Load each inventory slot
                for (int i = 1; i <= 5; i++)
                {
                    string line = reader.ReadLine();
                    Inventory[i] = line.Replace("Slot " + i + " = ", "");
                }

                reader.ReadLine();  // skip "--- Invis Inventory ---"

                // Load each slot in Invis Inventory
                for (int i = 1; i <= InvisibleInventory.Count; i++)
                {
                    string line = reader.ReadLine().Replace("Invis Slot " + i + " = ", "");
                    int divider = line.IndexOf('|');
                    string itemName = line.Substring(0, divider);
                    int quantity = Convert.ToInt32(line.Substring(divider + 1));
                    InvisibleInventory[i] = (itemName, quantity);
                }
                reader.Close();     //close reader
            }
            else
            {
                Console.WriteLine("\nAre you sure you want to overwrite save file? [y/n]");
                string temp = Console.ReadLine().ToLower();

                if (temp == "y")
                {
                    Directory.CreateDirectory(saveFolder);

                    string saveContents =
                        "Level = " + currentLevel + "\n" +
                        "X = " + x + "\n" +
                        "Y = " + y + "\n" +
                        "Health = " + hp + "\n" +
                        "Jason Enemy Dead = " + jasonEnemyDefeated + "\n" +
                        "Slimy Jack Enemy Dead = " + slimyJackEnemyDefeated + "\n\n" +
                        "--- Inventory ---\n" +
                        "Inventory = " + inventoryPosition + "\n";
                    // Save each slot
                    foreach (var item in Inventory)
                        {
                            saveContents += "Slot " + item.Key + " = " + item.Value + "\n";
                        }
                    saveContents += "--- Invis Inventory ---\n";
                    // Save each slot in Invis Inventory
                    foreach (var item in InvisibleInventory)
                    {
                        saveContents += "Invis Slot " + item.Key + " = " + item.Value.ItemName + "|" + item.Value.Quantity + "\n";
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

        static void PlayerAttack(string currentWeapon)
        {
            /*EXPLANATION: This is the method for the player's attack*/

            Console.SetCursorPosition(42, 2);
            Console.Write("The player has attacked the enemy!");

            Random rand = new Random();

            int upperLimit = 3, lowerLimit = 1;

            switch (currentWeapon) //this determines the maximum and minimum damage of the current weapon
            {
                case "Empty Handed":
                    upperLimit = 2;
                    lowerLimit = 1;
                    break;
                case "Wooden Sword":
                    upperLimit = 4;
                    lowerLimit = 2;
                    break;
                case "Iron Sword":
                    upperLimit = 6;
                    lowerLimit = 3;
                    break;
                case "Diamond Sword":
                    upperLimit = 8;
                    lowerLimit = 4;
                    break;
            }

            int attackPower = rand.Next(lowerLimit, upperLimit);

            if(enemyHP - attackPower < 0)
            {
                enemyHP = 0;
            }
            else
            {
                enemyHP -= attackPower;
            }

            Console.SetCursorPosition(50, 3);
            Console.BackgroundColor = ConsoleColor.Cyan;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write($" Damage dealt: {attackPower} ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;

            Console.Beep(100, 90);
            Console.Beep(200, 90);
            Console.Beep(300, 90);
            Console.Beep(400, 90);

            Thread.Sleep(2000);
        }

        static void PlayerBlock()
        {
            Console.SetCursorPosition(44, 2);
            Console.Write("   Utility action detected.   ");

            Console.SetCursorPosition(38, 3);
            Console.BackgroundColor = ConsoleColor.Cyan;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write($"The player will block the incoming attack!");
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;

            /*EXPLANATION: This is the method for the block utility*/

            enemyAttackModifier = 0;

            Console.Beep(100, 90);
            Console.Beep(200, 90);
            Console.Beep(300, 90);
            Console.Beep(400, 90);

            Thread.Sleep(2000);
        }

        static void PlayerHeal()
        {
            /*EXPLANATION: This is the method for the heal utility*/
        }

        static void EnemyAIReaction(int previousAction)
        {
            switch (previousAction)
            {
                case 0:
                    Console.SetCursorPosition(46, 2);
                    Console.Write("No player action detected.");
                    break;
                case 1:
                    EnemyAIAttack();
                    break;
                case 2:
                    EnemyAIAttack();
                    break;
                case 3:
                    EnemyAIAttack();
                    break;
                case 4:
                    Console.SetCursorPosition(46, 2);
                    Console.Write("You can't run away, punk!");
                    break;
                default:
                    Console.SetCursorPosition(42, 2);
                    Console.Write("An unexpected error has occurred.");
                    break;
            }

            if(previousAction != 0)
            {
                Console.Beep(400, 90);
                Console.Beep(300, 90);
                Console.Beep(200, 90);
                Console.Beep(100, 90);
            }

            enemyAttackModifier = 1;
        }

        static void EnemyAIAttack()
        {
            Console.SetCursorPosition(38, 2);
            Console.Write("   The enemy has attacked the player!   ");

            Random rand = new Random();
            double attackPower = rand.Next(1, 5);
            double totalDamage = attackPower * enemyAttackModifier;

            if(hp - attackPower < 0)
            {
                hp = 0;
            }
            else
            {
                hp -= totalDamage;
            }

            HP(hp);

            Console.SetCursorPosition(50, 3);
            Console.BackgroundColor = ConsoleColor.DarkYellow;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write($" Damage dealt: {totalDamage} ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
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
            Dictionary<int, int> Times = new Dictionary<int, int>()
            {
                [1] = 8000,
                [2] = 9500,
                [3] = 6000,
                [4] = 1500,
                [5] = 4000,
                [6] = 5500,
                [7] = 10500,
            };

            // Dictionary<PotionID, IngredientIDs[]>
            Dictionary<int, int[]> PotionRecipes = new Dictionary<int, int[]>()
            {
                { 11, new int[] { 7, 20, 17 } },  // Health Potion   -> Berries + Slime + Moss
                { 12, new int[] { 17, 18 } },     // Antidote        -> Moss + Spider Fang
                { 13, new int[] { 18, 20, 25 } }, // Poison Potion   -> Spider Fang + Slime + Bug
                { 14, new int[] { 24, 23 } },     // Strength Potion -> Cryptcrawler Root + Bone
                { 15, new int[] { 3, 17 } },      // Mana Potion     -> Crystal Shard + Moss
                { 16, new int[] { 7, 22, 10 } },  // Stamina Potion  -> Berries + Feather + Blind Cave Fish
                { 27, new int[] { 20, 21, 19 } }  // WD-40           -> Slime + Iron Shard + Stone
            };


            string[] CauldronLines = null;

            Console.Clear();
            string filePath = $"Assets/UIs/Cauldron.txt";
            if (File.Exists(filePath))
            {
                CauldronLines = File.ReadAllLines(filePath);
                Console.WriteLine(string.Join(Environment.NewLine, CauldronLines));
            }
            else
            {
                Console.WriteLine($"Error: Can't find the file: {filePath}");
            }

            Console.WriteLine("Choose a potion to brew: ");
            int potionChoice = Convert.ToInt32(Console.ReadLine());
            int brewTime = 0;
            int BrewedPotion = 0;
            bool AllIngredents = false;

            switch (potionChoice)
            {
                case 1:
                    BrewedPotion = 11;
                    AllIngredents = CheckNeededIngredients(Brewing, PotionRecipes[BrewedPotion]);
                    break;
                case 2:
                    BrewedPotion = 12;
                    AllIngredents = CheckNeededIngredients(Brewing, PotionRecipes[BrewedPotion]);
                    break;
                case 3:
                    BrewedPotion = 13;
                    AllIngredents = CheckNeededIngredients(Brewing, PotionRecipes[BrewedPotion]);
                    break;
                case 4:
                    BrewedPotion = 14;
                    AllIngredents = CheckNeededIngredients(Brewing, PotionRecipes[BrewedPotion]);
                    break;
                case 5:
                    BrewedPotion = 15;
                    AllIngredents = CheckNeededIngredients(Brewing, PotionRecipes[BrewedPotion]);
                    break;
                case 6:
                    BrewedPotion = 16;
                    AllIngredents = CheckNeededIngredients(Brewing, PotionRecipes[BrewedPotion]);
                    break;
                case 7:
                    BrewedPotion = 27;
                    AllIngredents = CheckNeededIngredients(Brewing, PotionRecipes[BrewedPotion]);
                    break;
                default:
                    Console.WriteLine("Invalid choice. No potion brewed.");
                    break;
            }
            if (AllIngredents)
            {
                brewTime = Times[potionChoice];
                int row = Array.FindIndex(CauldronLines, l => l.Contains("PROGRESS:"));
                int left = CauldronLines[row].IndexOf('[') + 1;
                int width = CauldronLines[row].IndexOf(']') - left;
                Timer(brewTime, width, left, row);
                Interaction(BrewedPotion);
            }
        }

        static void Timer(int totalMs, int width, int left, int row)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;  // needed for ░▒▓█

            for (int pct = 0; pct <= 100; pct++)  // loop from 0 to 100 percent
            {
                char[] bar = new char[width];
                int filled = pct * width / 100;     // calculate how many characters should be filled in the bar
                for (int i = 0; i < width; i++)
                {
                    if (i < filled)
                    {
                        bar[i] = '█';
                    }
                    else
                    {
                        bar[i] = '░';
                    }
                }

                string label = $" {pct}% ";     // label (percentage done) to display in the middle of the bar
                int start = (width - label.Length) / 2;     // starting position of the label
                label.CopyTo(0, bar, start, label.Length); // copy the label into the bar array

                Console.SetCursorPosition(left, row);   // move the cursor to the start of the bar
                Console.Write(new string(bar));
                Thread.Sleep(totalMs / 100);
            }
        }

        static bool CheckNeededIngredients(Action brewing, int[] required)
        {
            // First pass: make sure every ingredient is available in the amount needed before using anything
            foreach (int id in required)
            {
                string name = Items[id];

                int amountNeeded = 0;
                for (int i = 0; i < required.Length; i++)
                {
                    if (required[i] == id)
                    {
                        amountNeeded++;
                    }
                }

                bool found = false;

                for (int slot = 1; slot <= InvisibleInventory.Count; slot++)
                {
                    if (InvisibleInventory[slot].ItemName == name && InvisibleInventory[slot].Quantity >= amountNeeded)
                    {
                        found = true;
                    }
                }

                if (!found)
                {
                    Console.Write($"You don't have the required ingredient: {name} (need {amountNeeded}).\n");
                    return false;
                }
            }

            // Everything was found, consume the ingredients
            foreach (int id in required)
            {
                string name = Items[id];

                for (int slot = 1; slot <= InvisibleInventory.Count; slot++)
                {
                    if (InvisibleInventory[slot].ItemName == name && InvisibleInventory[slot].Quantity > 0)
                    {
                        int newQuantity = InvisibleInventory[slot].Quantity - 1;

                        if (newQuantity > 0)
                        {
                            InvisibleInventory[slot] = (name, newQuantity);
                        }
                        else
                        {
                            InvisibleInventory[slot] = (Items[1], 0); // back to "Nothing"
                        }
                    }
                }
            }
            return true;
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
        static void WriteLineCenter(string text, int textY)
        {
            Console.SetCursorPosition(0, textY);
            if (text.Length >= Console.WindowWidth)
            {
                Console.WriteLine(text);
            }
            else
            {
                int space = (Console.WindowWidth - text.Length)/2;
                Console.WriteLine(new string(' ', space) + text);
            }
        }



        //Temporary methods to add items to invisible inventory, and to check stacking

        static void AddToInvisibleInventory(int itemId)
        {
            // Get the item name from your main Items dictionary
            string itemName = Items[itemId];

            for (int slot = 1; slot <= InvisibleInventory.Count; slot++)
            {
                if (InvisibleInventory[slot].ItemName == itemName)
                {
                    // Update the quantity directly in place using the index variable
                    InvisibleInventory[slot] = (itemName, InvisibleInventory[slot].Quantity + 1);
                    Console.WriteLine($"Stacked {itemName}. New quantity: {InvisibleInventory[slot].Quantity}");
                    return;
                }
            }

            // If it doesn't exist, find the first empty slot (where name is "Nothing")
            for(int slot = 1; slot <= InvisibleInventory.Count; slot++)
            {
                if (InvisibleInventory[slot].ItemName == Items[1]) // Items[1] is "Nothing"
                {
                    InvisibleInventory[slot] = (itemName, 1);
                    Console.WriteLine($"Added {itemName} to invisible slot {slot}.");
                    return;
                }
            }

            Console.WriteLine("Invisible inventory is completely full!");
        }

    }
}
