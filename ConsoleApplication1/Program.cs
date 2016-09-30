
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

        
        

        // game loop
        while (true)
        {

            // Base Grid
            char[][] grid = new char[height][];
            for (int i = 0; i < height; i++) grid[i] = new char[width];

            // Bomb Value Grid
            char[][] gridValue = new char[height][];
            for (int i = 0; i < height; i++) gridValue[i] = new char[width];

            // Walkable Grid
            char[][] gridWalkable = new char[height][];
            for (int i = 0; i < height; i++) gridValue[i] = new char[width];

            List<Bomb> bombs = new List<Bomb>();

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
                //else if (entityType == 2) items[owner].Update(x, y, param1, param2);

            }




            gridValue = Calcs.BombValueGrid(grid, players[myId].GetBombRange());
            gridWalkable = Calcs.WalkableGrid(gridValue, players[myId].GetX(), players[myId].GetY());
            //Calcs.DebugDisplayGrid(grid);
            //Calcs.DebugDisplayGrid(gridValue);
            foreach (Bomb bomb in bombs)
            {
                gridValue = Calcs.AddBombExplosion(gridValue, bomb.GetX(), bomb.GetY(), bomb.GetRange());
            }


            

            //Calcs.DebugDisplayGrid(gridWalkable);
            Calcs.DebugDisplayGrid(gridValue);
            //Console.Error.WriteLine("myId: " + myId);
            //Console.Error.WriteLine("myX: " + players[myId].GetX());
            //Console.Error.WriteLine("myY: " + players[myId].GetY());

            // Find location for next bomb
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {

                    //Console.Error.WriteLine("gridValue["+i+"]["+j+"]: " + gridValue[i][j]);
                    //Console.Error.WriteLine("Valid? " + Calcs.ValidBomb(gridValue, gridWalkable, j, i, players[myId].GetBombRange()));

                    if (gridValue[i][j] > value && gridValue[i][j] != 'X')
                    {
                        if ((Calcs.ValidBomb(gridValue, gridWalkable, j, i, players[myId].GetBombRange()) == true && players[myId].GetBombNum() > 0) ||
                            (players[myId].GetBombNum() == 0 && gridWalkable[i][j] == 'W' && gridValue[i][j] != 'X'))
                        {

                            Console.Error.WriteLine("gridValue[i][j]: " + gridValue[i][j]);
                            //Console.Error.WriteLine("j/x: " +j);
                            //Console.Error.WriteLine("players[myId].GetY(): " + players[myId].GetY());
                            //Console.Error.WriteLine("players[myId].GetX(): " + players[myId].GetX());
                            //Console.Error.WriteLine((bool)(players[myId].GetX() == j && players[myId].GetY() == i));

                            output = "MOVE " + j + " " + i;

                            if (players[myId].GetX() == j && players[myId].GetY() == i)
                            {
                                output = "BOMB " + j + " " + i;
                            }
                            
                            value = gridValue[i][j];
                        }
                    }
                }
            }
            

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
                if (grid[i][x] != 'X') output += (int)Char.GetNumericValue(grid[i][x])+1;
                break;
            }
        }

        // Right
        for (int i = Math.Min(x + 1, grid[y].Length); i < Math.Min(x + bombRange, grid[y].Length); i++)
        {
            if (grid[y][i] != '.')
            {
                if (grid[y][i] != 'X') output += (int)Char.GetNumericValue(grid[y][i])+1;
                break;
            }
        }

        // Up
        for (int i = Math.Max(y - 1, 0); i > Math.Max(y - bombRange, -1); i--)
        {
            if (grid[i][x] != '.')
            {
                if (grid[i][x] != 'X') output += (int)Char.GetNumericValue(grid[i][x])+1;
                break;
            }
        }

        // Left
        for (int i = Math.Max(x - 1, 0); i > Math.Max(x - bombRange, -1); i--)
        {
            if (grid[y][i] != '.')
            {
                if (grid[y][i] != 'X') output += (int)Char.GetNumericValue(grid[y][i])+1;
                break;
            }
        }

        if (output > 9) output = 9;

        return output;

    }



    // Add Bomb explosion as Xs to grid
    public static char[][] AddBombExplosion(char[][] grid, int x, int y, int bombRange)
    {
        char[][] output = CopyArrayLinq(grid);

        // Down
        for (int i = Math.Min(y, grid.Length - 1); i < Math.Min(y + bombRange, grid.Length); i++)
        {
            if (grid[i][x] == 'X') break;
            output[i][x] = 'X';
        }

        // Right
        for (int i = Math.Min(x + 1, grid[y].Length); i < Math.Min(x + bombRange, grid[y].Length); i++)
        {
            if (grid[y][i] == 'X') break;
            output[y][i] = 'X';
        }

        // Up
        for (int i = Math.Max(y - 1, 0); i > Math.Max(y - bombRange, -1); i--)
        {
            if (grid[i][x] == 'X') break;
            output[i][x] = 'X';
        }

        // Left
        for (int i = Math.Max(x - 1, 0); i > Math.Max(x - bombRange, -1); i--)
        {
            if (grid[y][i] == 'X') break;
            output[y][i] = 'X';
        }

        return output;
    }



    // Calc Bomb Value Table
    public static char[][] BombValueGrid(char[][] grid, int bombRange)
    {

        char[][] output = new char[grid.Length][];
        for (int i = 0; i < grid.Length; i++) output[i] = new char[grid[i].Length];

        for (int i = 0; i < grid.Length; i++)
        {
            for (int j = 0; j < grid[i].Length; j++)
            {
                if (grid[i][j] == '.')
                {
                    output[i][j] = char.Parse(Calcs.BombValue(j, i, bombRange, grid).ToString());
                }
                else
                {
                    output[i][j] = 'X';
                }
            }
        }

        return output;
    }



    // Check if bomb location is valid
    public static bool ValidBomb(char[][] gridV, char[][] gridWalkable, int x, int y, int bombRange)
    {
        if (gridWalkable[y][x] == 'W')
        {
            //Console.Error.WriteLine("x: " +x);
            //Console.Error.WriteLine("y: " +y);
            

            gridWalkable = CopyArrayLinq(AddBombExplosion(gridWalkable, x, y, bombRange));

            //DebugDisplayGrid(gridWalkable);

            for (int i = 0; i < gridWalkable.Length; i++)
            {
                for (int j = 0; j < gridWalkable[i].Length; j++)
                {
                    if (gridWalkable[i][j] == 'W')
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }


    
    // Check Walkable Area
    public static char[][] WalkableGrid(char[][] gridV, int x, int y)
    {
        char[][] output = new char[gridV.Length][];
        for (int i = 0; i < gridV.Length; i++) output[i] = new char[gridV[i].Length];

        output = CopyArrayLinq(gridV);

        if (x > output[1].Length-1 || x < 0) return output;
        else if (y > output.Length-1 || y < 0) return output;


        if (output[y][x] != 'X' && output[y][x] != 'W')
        {
            output[y][x] = 'W';
            output = WalkableGrid(output, x + 1, y);
            output = WalkableGrid(output, x - 1, y);
            output = WalkableGrid(output, x, y + 1);
            output = WalkableGrid(output, x, y - 1);
        }

        return output;
    }


    // Debug Display Grid
    public static void DebugDisplayGrid(char[][] grid)
    {
        Console.Error.WriteLine("Debug Table");
        for (int i = 0; i < grid.Length; i++)
        {
            for (int j = 0; j < grid[i].Length; j++)
            {
                Console.Error.Write(grid[i][j]);
            }
            Console.Error.WriteLine("");
        }
    }


    // Copy Jagged Array
    static char[][] CopyArrayLinq(char[][] source)
    {
        return source.Select(s => s.ToArray()).ToArray();
    }

}