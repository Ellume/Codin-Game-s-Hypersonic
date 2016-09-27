using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

/**
 * Game initialization and turn loop
 * 
 **/
class Game
{
    static void Main(string[] args)
    {
        string[] inputs;
        inputs = Console.ReadLine().Split(' ');
        int width = int.Parse(inputs[0]);
        int height = int.Parse(inputs[1]);
        int myId = int.Parse(inputs[2]);

        Player[] players = new Player[4];
        for (int i = 0; i < 4; i++) players[i] = new Player();

        List<Bomb> bombs = new List<Bomb>();

        // Base Grid
        char[][] grid = new char[height][];
        for (int i = 0; i < height; i++) grid[i] = new char[width];

        // Bomb Value Grid
        char[][] gridValue = new char[height][];
        for (int i = 0; i < height; i++) gridValue[i] = new char[width];


        // game loop
        while (true)
        {
            string output = "";
            int value = 0;

            // Update grid
            for (int i = 0; i < height; i++)
            {
                grid[i] = Console.ReadLine().ToCharArray();
            }

            // Update players & bombs
            bombs.Clear();
            int entities = int.Parse(Console.ReadLine());
            for (int i = 0; i < entities; i++)
            {
                inputs = Console.ReadLine().Split(' ');
                int entityType = int.Parse(inputs[0]);
                int owner = int.Parse(inputs[1]);
                int x = int.Parse(inputs[2]);
                int y = int.Parse(inputs[3]);
                int param1 = int.Parse(inputs[4]);
                int param2 = int.Parse(inputs[5]);

                //if (param1 == 8) Console.Error.WriteLine("WOOOOOOOOOOOOOOO!!!!");
                // Console.Error.WriteLine("

                if (entityType == 0) players[owner].Update(x, y, param1, param2);
                else if (entityType == 1)
                {
                    bombs.Add(new Bomb());
                    bombs[bombs.Count - 1].Update(x, y, param1, param2);
                }
                else if (entityType == 2) players[owner].Update(x, y, param1, param2);

            }

            // Find location for next bomb
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (Calcs.BombValue(j, i, 3, grid) > value && grid[i][j] != '0')
                    {
                        output = "BOMB " + j + " " + i;
                        value = Calcs.BombValue(j, i, 3, grid);
                    }
                }
            }


            gridValue = Calcs.BombValueTable(grid, 3);

            Calcs.DebugDisplayGrid(grid);
            Calcs.DebugDisplayGrid(gridValue);


            Console.WriteLine(output);
        }
    }
}




/**
 * Player object
 * 
 **/
class Player
{
    int x;
    int y;
    int bombNum;
    int bombRange;

    public void Update(int X, int Y, int PARAM1, int PARAM2)
    {
        x = X;
        y = Y;
        bombNum = PARAM1;
        bombRange = PARAM2;
    }

    public int GetX()
    {
        return x;
    }

    public int GetY()
    {
        return y;
    }

    public int GetBombNum()
    {
        return bombNum;
    }

    public int GetBombRange()
    {
        return bombRange;
    }
}



/**
 * Bomb object
 * 
 **/
class Bomb
{
    int x;
    int y;
    int bombTimer;
    int bombRange;

    public void Update(int X, int Y, int PARAM1, int PARAM2)
    {
        x = X;
        y = Y;
        bombTimer = PARAM1;
        bombRange = PARAM2;
    }

    public int GetX()
    {
        return x;
    }

    public int GetY()
    {
        return y;
    }

    public int GetTimer()
    {
        return bombTimer;
    }

    public int GetRange()
    {
        return bombRange;
    }
}



/**
 * Item object
 * 
 **/
class Item
{
    int x;
    int y;
    int itemType;
    int itemUnknown;

    public void Update(int X, int Y, int PARAM1, int PARAM2)
    {
        x = X;
        y = Y;
        itemType = PARAM1;
        itemUnknown = PARAM2;
    }

    public int GetX()
    {
        return x;
    }

    public int GetY()
    {
        return y;
    }

    public int GetItemType()
    {
        return itemType;
    }

    public int GetUnknown()
    {
        return itemUnknown;
    }
}



/**
 * Calculation methods
 * 
 **/
class Calcs
{

    // Check for value of items in boxes
    public static int BombValue(int x, int y, int bombRange, char[][] grid)
    {
        int output = 0;

        // Down
        for (int i = Math.Min(y + 1, grid.Length); i < Math.Min(y + bombRange, grid.Length); i++)
        {
            if (grid[i][x] != '.')
            {
                if (grid[i][x] != 'X') output += (int)Char.GetNumericValue(grid[i][x]);
                break;
            }
        }

        // Right
        for (int i = Math.Min(x + 1, grid[y].Length); i < Math.Min(x + bombRange, grid[y].Length); i++)
        {
            if (grid[y][i] != '.')
            {
                if (grid[y][i] != 'X') output += (int)Char.GetNumericValue(grid[y][i]);
                break;
            }
        }

        // Up
        for (int i = Math.Max(y - 1, 0); i > Math.Max(y - bombRange, 0); i--)
        {
            if (grid[i][x] != '.')
            {
                if (grid[i][x] != 'X') output += (int)Char.GetNumericValue(grid[i][x]);
                break;
            }
        }

        // Left
        for (int i = Math.Max(x - 1, 0); i > Math.Max(x - bombRange, 0); i--)
        {
            if (grid[y][i] != '.')
            {
                if (grid[y][i] != 'X') output += (int)Char.GetNumericValue(grid[y][i]);
                break;
            }
        }

        return output;

    }





    // Calc Bomb Value Table
    public static char[][] BombValueTable(char[][] grid, int bombRange)
    {

        char[][] output = new char[grid.Length][];
        for (int i = 0; i < grid.Length; i++) output[i] = new char[grid[i].Length];

        for (int i = 0; i < grid.Length; i++)
        {
            for (int j = 0; j < grid[i].Length; j++)
            {
                if (grid[i][j] == '.')
                {
                    output[i][j] = Convert.ToChar(Calcs.BombValue(j, i, bombRange, grid));
                    //Console.Error.WriteLine(Calcs.BombValue(j, i, bombRange, grid));
                }
                else
                {
                    output[i][j] = 'X';
                }
            }
        }

        return output;
    }



    // Debug Display Grid
    public static void DebugDisplayGrid(char[][] grid)
    {
        for (int i = 0; i < grid.Length; i++)
        {
            for (int j = 0; j < grid[i].Length; j++)
            {
                Console.Error.Write(grid[i][j]);
            }
            Console.Error.WriteLine("");
        }
    }

}