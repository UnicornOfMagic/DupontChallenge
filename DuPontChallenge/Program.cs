using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuPontChallenge
{
    class Program
    {

        class Mirror {
            public int x = 0;
            public int y = 0;
            public string direction = "";
            public string type = "";

            public Mirror(int x, int y, string direction, string type)
            {
                this.x = x;
                this.y = y;
                this.direction = direction;
                this.type = type;
                if(type.Equals(""))
                {
                    this.type = "2";
                }
            }

            override public String ToString()
            {
                string output = "";

                output = "x:" + x + ",y:" + y + ",d:" + direction + ",t:" + type;

                return output;
            }

        }

        class Lazer
        {
            public int x = 0;
            public int y = 0;
            public int xVel = 0;
            public int yVel = 0;
            public string orientation = "";

            public Lazer() { }

            public Lazer(int x, int y, string orientation)
            {
                this.x = x;
                this.y = y;
                this.orientation = orientation;
            }

            public Lazer(int x, int y, int xVel, int yVel, string orientation)
            {
                this.x = x;
                this.y = y;
                this.xVel = xVel;
                this.yVel = yVel;
                this.orientation = orientation;
            }

            override public string ToString()
            {
                var output = "";

                output = "x:" + x + ",y:" + y + ",o:" + orientation;

                return output;
            }
            
            public override bool Equals(Object obj)
            {
                var lazer = obj as Lazer;

                if (lazer == null)
                {
                    return false;
                }
                
                return this.x == lazer.x 
                    && this.y == lazer.y 
                    && this.xVel == lazer.xVel 
                    && this.yVel == lazer.yVel; //if all attributes match then they're equal
            }


            public void TurnRight()
            {
                if(orientation.Equals("H"))
                {
                    orientation = "V";
                } else
                {
                    orientation = "H";
                }

                if (xVel != 0)
                {
                    if (xVel > 0)
                    {
                        yVel = -1;
                    } else
                    {
                        yVel = 1;
                    }
                    xVel = 0;
                } else if(yVel != 0)
                {
                    if (yVel > 0)
                    {
                        xVel = 1;
                    } else
                    {
                        xVel = -1;
                    }
                    yVel = 0;
                }
            }

            public void TurnLeft()
            {
                if (orientation.Equals("H"))
                {
                    orientation = "V";
                }
                else
                {
                    orientation = "H";
                }

                if (xVel != 0)
                {
                    if (xVel > 0)
                    {
                        yVel = 1;
                    }
                    else
                    {
                        yVel = -1;
                    }
                    xVel = 0;
                }
                else if (yVel != 0)
                {
                    if (yVel > 0)
                    {
                        xVel = -1;
                    }
                    else
                    {
                        xVel = 1;
                    }
                    yVel = 0;
                }
            }
        }


        static void Main(string[] args)
        {
            Lazer lazer;
            List<Mirror> mirrorPositions;
            int boardSizeX, boardSizeY;
            Setup(out lazer, out mirrorPositions, out boardSizeX, out boardSizeY); //setup our initial values and such we'll be working with


            Mirror[,] board = BuildBoard(mirrorPositions, boardSizeX, boardSizeY); //build the board

            List<Lazer> history;
            bool error;
            lazer = FireLazer(lazer, boardSizeX, boardSizeY, board, out history, out error); //main logic of the program

            //Output the results
            Console.WriteLine("1. (" + boardSizeX + ", " + boardSizeY + ")");
            Console.WriteLine("2. (" + history.First().x + ", " + history.First().y + ")" + history.First().orientation);
            if (error)
            {
                Console.WriteLine("3. It didn't.");
            }
            else
            {
                Console.WriteLine("3. (" + lazer.x + ", " + lazer.y + ")" + lazer.orientation);
            }
        }

        private static void Setup(out Lazer lazer, out List<Mirror> mirrorPositions, out int boardSizeX, out int boardSizeY)
        {
            var lines = System.IO.File.ReadAllLines(@"input\input.txt");
            StringBuilder sb = new StringBuilder();
            lazer = new Lazer();
            mirrorPositions = new List<Mirror>();
            boardSizeX = 0;
            boardSizeY = 0;
            var count = 0;
            foreach (var line in lines)
            {
                if (line.Equals("-1"))
                {
                    count++; //consume the -1
                }
                else {
                    switch (count) //based on how many -1s we have seen, we know what type of data we are fed
                    {
                        case 0: //Size of board is first
                            var boardData = line.Split(',');
                            boardSizeX = int.Parse(boardData[0]);
                            boardSizeY = int.Parse(boardData[1]);
                            break;
                        case 1: //Then the mirror(s) position, orientation, and type...
                            var mirrorX = 0;
                            var mirrorY = 0;
                            var direction = "";
                            var type = "";
                            var mirrorData = line.Split(',');
                            mirrorX = int.Parse(mirrorData[0]);
                            if (int.TryParse(mirrorData[1].Remove(mirrorData[1].Length - 1), out mirrorY)) //if TryParse works then the mirror is two-way (no last char)
                            {
                                direction = mirrorData[1][(mirrorData[1].Length - 1)] + ""; //extract the last char since we know there isn't a second one denoting one-way direction
                            }
                            else //if it fails that means we passed a char into the try parse meaning we have 2 chars at the end
                            {
                                mirrorY = int.Parse(mirrorData[1].Remove(mirrorData[1].Length - 2)); //so we trim the last 2 chars for the y coordinates
                                direction = mirrorData[1][(mirrorData[1].Length - 2)] + "";    //then the direction should be the 2nd to last character
                                type = mirrorData[1][(mirrorData[1].Length - 1)] + "";         //and the "type" is the last character and it is one-way (should be 'L' or 'R')
                            }
                            mirrorPositions.Add(new Mirror(mirrorX, mirrorY, direction, type));//add our new mirror
                            break;
                        case 2: //Finally the laser's entry point and orientation
                            var lazerX = 0;
                            var lazerY = 0;
                            var orientation = "";
                            var lazerData = line.Split(',');
                            lazerX = int.Parse(lazerData[0]);
                            lazerY = int.Parse(lazerData[1].Remove(lazerData[1].Length - 1));
                            orientation = lazerData[1][lazerData.Length - 1] + "";
                            lazer = new Lazer(lazerX, lazerY, orientation);
                            break;
                    }
                }
            }

            //We need to determine our lazer's initial velocity
            if (lazer.orientation.Equals("H")) //coming from either left or right
            {
                if (lazer.x == 0) //from left
                {
                    lazer.xVel = 1;
                }
                else //from right
                {
                    lazer.xVel = -1;
                }
            }
            else //coming from either top or bottom
            {
                if (lazer.y == 0) //from bottom
                {
                    lazer.yVel = 1;
                }
                else //from top
                {
                    lazer.yVel = -1;
                }
            }
        }

        private static Mirror[,] BuildBoard(List<Mirror> mirrorPositions, int boardSizeX, int boardSizeY)
        {

            //Build the board!!
            var board = new Mirror[boardSizeY, boardSizeX];
            for (int i = 0; i < boardSizeY; i++)
            {
                for (int j = 0; j < boardSizeX; j++)
                {
                    board[i, j] = null; //fill board with nulls. Null means no mirror!
                }
            }
            //add our mirrors
            foreach (var mirror in mirrorPositions)
            {
                board[mirror.y, mirror.x] = mirror;
            }

            return board;
        }

        private static Lazer FireLazer(Lazer lazer, int boardSizeX, int boardSizeY, Mirror[,] board, out List<Lazer> history, out bool error)
        {
            bool done = false;
            history = new List<Lazer>();
            history.Add(new Lazer(lazer.x, lazer.y, lazer.xVel, lazer.yVel, lazer.orientation));
            error = false;
            while (!done) //main logic loop for running the lazer through the maze
            {
                //check if we hit a mirror
                var obstacle = board[lazer.y, lazer.x];
                if (obstacle != null)
                {
                    var mirrorDirection = obstacle.direction;
                    var mirrorType = obstacle.type;
                    if (mirrorDirection.Equals("L")) //Time to bounce
                    {
                        if (lazer.xVel > 0) //going right
                        {
                            if (mirrorType.Equals("2") || mirrorType.Equals("L"))
                            {
                                lazer.TurnRight();
                            }
                        }
                        else if (lazer.yVel > 0) //going up
                        {
                            if (mirrorType.Equals("2") || mirrorType.Equals("L"))
                            {
                                lazer.TurnLeft();
                            }
                        }
                        else if (lazer.xVel < 0) //going left
                        {
                            if (mirrorType.Equals("2") || mirrorType.Equals("R"))
                            {
                                lazer.TurnRight();
                            }
                        }
                        else //going down
                        {
                            if (mirrorType.Equals("2") || mirrorType.Equals("R"))
                            {
                                lazer.TurnLeft();
                            }
                        }
                    }
                    else
                    {
                        if (lazer.xVel > 0) //going right
                        {
                            if (mirrorType.Equals("2") || mirrorType.Equals("L"))
                            {
                                lazer.TurnLeft();
                            }
                        }
                        else if (lazer.yVel > 0) //going up
                        {
                            if (mirrorType.Equals("2") || mirrorType.Equals("R"))
                            {
                                lazer.TurnRight();
                            }
                        }
                        else if (lazer.xVel < 0) //going left
                        {
                            if (mirrorType.Equals("2") || mirrorType.Equals("R"))
                            {
                                lazer.TurnLeft();
                            }
                        }
                        else //going down
                        {
                            if (mirrorType.Equals("2") || mirrorType.Equals("L"))
                            {
                                lazer.TurnRight();
                            }
                        }
                    }
                }
                lazer.x += lazer.xVel;
                lazer.y += lazer.yVel;

                //check if we exited
                if (lazer.x < 0 || lazer.x > boardSizeX || lazer.y < 0 || lazer.y > boardSizeY)
                {
                    done = true;
                }
                else if (history.Contains(lazer))
                {
                    error = true; //infinite loop detected
                    done = true;
                }
                else
                {
                    history.Add(new Lazer(lazer.x, lazer.y, lazer.xVel, lazer.yVel, lazer.orientation));
                }
            } //end while
            
            return history.Last(); //use last lazer before exiting for reporting purposes
        }
    }
}
