using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Game
{
    class Program
    {
        static void Main(string[] args)
        {
            string [] gameItems = args;
            bool result = ValidationItems(gameItems);
            Random rnd = new Random();
            if(result == true)
            {
                List<string> variables = new List<string>(gameItems);
                int computerMove = rnd.Next(0, variables.Count - 1);
                byte[] bkeys = getKey();
                string key = getHash(variables[computerMove].ToString(), bkeys);
                Console.WriteLine($"HMAC: {key}");
                string move = ValidationMove(variables);
                if (move == "0")
                    return;
                Game(int.Parse(move), computerMove, variables);
                Console.WriteLine($"HMAC KEY:{BitConverter.ToString(bkeys).Replace("-", string.Empty).ToUpper()}");
            }
        }
        static bool ValidationItems(string[] gameItems)
        {
            var unicItems = new HashSet<string>(gameItems);
                if (gameItems.Length < 3)
                {
                    Console.WriteLine("Wrong input, please write more unless 3 parameters. Try this: 1 2 4 5 6 7 8");
                    return false;
                }
                if (gameItems.Length % 2 == 0)
                {
                    Console.WriteLine("Wrong! Enter an odd number of parameters.");
                    return false;
                }
                if (unicItems.Count != gameItems.Length)
                {
                    Console.WriteLine("Wrong input, please write unic items. Try this: 1 2 4 5 6 7 8");
                    return false;
                }
                if (gameItems.Length == 0)
                {
                    Console.WriteLine("Wrong input, please write something. Try this: 1 2 4 5 6 7 8");
                    return false;
                }
                return true;
        }
        static string ValidationMove(List<string> variables)
        {
            Menu(variables);
            string move = Console.ReadLine();
            if (move == "0")
                return "0";
            bool result = false;
            while(!result)
            {
                if(int.Parse(move) > variables.Count)
                {
                    Console.WriteLine("Wrong move, please be more attentively!");
                    Menu(variables);
                    result = false;
                    move = Console.ReadLine();
                }
                else
                {
                    result = true;
                }
            }
            Console.WriteLine($"Your move: {variables[int.Parse(move) - 1]}");
            return move;
        }
        static void Game (int playerMove, int enemyMove, List<string> variables)
        {
            int computerMove = enemyMove;
            Console.WriteLine($"Combuter move: {variables[computerMove]}");
            List<string> wins= new List<string>();
            var i = playerMove + 1;
            var count = 0;
            bool result = true;
            int winsLength = (variables.Count - 1) / 2;
            if (computerMove == playerMove - 1)
            {
                Console.WriteLine("It's a draw!");
                return;
            }
            while (count < winsLength)
            {
                count++;
                if (i >= variables.Count)
                    i = 0;
                wins.Add(variables[i]);
                i++;
            }
            foreach(string item in wins)
            {
                if(variables[computerMove] == item)
                {
                    Console.WriteLine("You win!");
                    result = false;
                }
            }
            if(result == true)
            {
                Console.WriteLine("You lose!");
            }
        }
        static byte[] getKey()
        {
            byte[] buffer = new byte[16];
            using(var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(buffer);
            }
            return buffer;
        }

        static string getHash (string str, byte[] key)
        {
           using (var hmac = new HMACSHA256(getKey()))
            {
                byte[] bstr = Encoding.Default.GetBytes(str);
                var bhash = hmac.ComputeHash(bstr);
                return BitConverter.ToString(bhash).Replace("-", string.Empty).ToUpper();
            }
        }
        static void Menu (List<string> variables)
        {
            Console.WriteLine("Available moves:");
            foreach (string item in variables)
            {
                Console.WriteLine($"{variables.IndexOf(item) + 1} - {item}");
            }
            Console.WriteLine("0 - exit");
            Console.Write("Enter your move: ");
        }
    }
}
