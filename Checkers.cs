using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WPF_Game_Checkers
{
    class Checkers
    {
        //default values
        private int xSize = 8, ySize = 8;
        public int numOfStoneRows = 2;
        public bool[,] validPosition;
        public Ellipse[,] playersLocation;
        Grid gameGrid;
        public bool freeMovement = false, whiteMove = false;
        public int attempsForMove = 0, moves = 0;

        public Checkers(Grid gameGrid)
        {
            this.gameGrid = gameGrid;
        }

        public int XSize
        {
            get { return xSize; }
            set { xSize = value; }
        }
        public int YSize
        {
            get { return ySize; }
            set { ySize = value; }
        }

        public int NumOfStoneRows
        {
            get { return numOfStoneRows; }
            set { numOfStoneRows = value; }
        }

        public void CreateGameGrid() //Crete grid for the game content.
        {
            //Initialize arrays
            validPosition = new bool[XSize,YSize];
            playersLocation = new Ellipse[XSize, YSize];

            for (int y = 0; y < YSize; y++)
            {
                RowDefinition row = new RowDefinition();
                gameGrid.RowDefinitions.Add(row);
            }

            for (int x = 0; x < XSize; x++)
            {
                ColumnDefinition colum = new ColumnDefinition();
                gameGrid.ColumnDefinitions.Add(colum);
            }
        }

        public void RenderCheckeredBackground()
        {
            bool oddRectangle = true;

            for (int y = 0; y < YSize; y++)
            {
                for (int x = 0; x < XSize; x++)
                {
                    validPosition[y, x] = !oddRectangle;

                    Rectangle rectangle = new Rectangle { Fill = oddRectangle ? Brushes.LightGray : Brushes.SaddleBrown }; //one is gray other is brown and so on...
                    rectangle.SetValue(Grid.RowProperty, y);
                    rectangle.SetValue(Grid.ColumnProperty, x);
                    gameGrid.Children.Add(rectangle);

                    oddRectangle = OddChecker(oddRectangle, x); //Creating the checkboard pattern
                }
            }
        }

        public void RenderStones(Style stoneStyle)
        {
            bool blackPlane = false, player2render = false;

            for (int y = 0; y < YSize; y++)
            {
                for (int x = 0; x < XSize; x++)
                {
                    //Render stones only on brown spots
                    if (blackPlane)
                    {
                        if (y < numOfStoneRows) //render for player 1 (at top)
                        {
                            Ellipse newPlayerStone = new Ellipse { Fill = Brushes.White, Margin = new Thickness(10) };
                            newPlayerStone.SetValue(Grid.RowProperty, y);
                            newPlayerStone.SetValue(Grid.ColumnProperty, x);
                            newPlayerStone.Name = $"white_{x}_{y}";
                            newPlayerStone.Style = stoneStyle; //Apply style with event handler

                            gameGrid.Children.Add(newPlayerStone); //Adding player's stone to be rendered in gameGrid
                            playersLocation[y, x] = newPlayerStone;
                        }

                        else //render for player 2 (at bottom)
                        {
                            if (!player2render)
                            {
                                y = YSize - numOfStoneRows;
                                player2render = true;
                            }

                            Ellipse newPlayerStone = new Ellipse { Fill = Brushes.Black, Margin = new Thickness(10) };
                            newPlayerStone.SetValue(Grid.RowProperty, y);
                            newPlayerStone.SetValue(Grid.ColumnProperty, x);
                            newPlayerStone.Name = $"black_{x}_{y}";
                            newPlayerStone.Style = stoneStyle; //Apply style with event handler

                            gameGrid.Children.Add(newPlayerStone); //Adding player's stone to be rendered in gameGrid
                            playersLocation[y, x] = newPlayerStone;
                        }
                    }
                    else
                        playersLocation[y, x] = null;

                    blackPlane = OddChecker(blackPlane, x); //Set if the checkboard plane is black
                }
            }
        }

        private bool OddChecker(bool oddRectangle, int x) //Return true only when last plate was false
        {
            oddRectangle = !oddRectangle;
            if (x == XSize - 1 && XSize % 2 == 0) //only true when x(rows) are even
                oddRectangle = !oddRectangle;

            return oddRectangle;
        }

        public bool MoveOneWayOnly(FrameworkElement lastPostion, FrameworkElement currentPosition)
        {
            int yLast = 0, yNow = 0;

            yLast = Grid.GetRow(lastPostion);
            yNow = Grid.GetRow(currentPosition);

            if (lastPostion.Name.Substring(0, 5) == "white")
            {
                if ((yNow - yLast) == 1)
                    return true;

                else if ((yNow - yLast) == 2)
                    return CheckKill(lastPostion, currentPosition);
            }

            else
            {
                if ((yNow - yLast) == -1)
                    return true;

                else if ((yNow - yLast) == -2)
                    return CheckKill(lastPostion, currentPosition);
            }

            return false;
        }

        private bool CheckKill(FrameworkElement lastPosition, FrameworkElement currentPosition)
        {
            int xVictim = 0, yVictim = 0;

            yVictim = (Grid.GetRow(lastPosition) + Grid.GetRow(currentPosition)) / 2;
            xVictim = (Grid.GetColumn(lastPosition) + Grid.GetColumn(currentPosition)) / 2;

            if (playersLocation[yVictim, xVictim] != null && !freeMovement)
            {
                var victim = gameGrid.Children.Cast<FrameworkElement>().Last(value => Grid.GetRow(value) == yVictim && Grid.GetColumn(value) == xVictim); //Find UIElement by row/culumn values. ".Last" because Ellipses were created after rectangles(checkerboard)

                if (victim.Name.Substring(0, 5) == lastPosition.Name.Substring(0, 5)) //Check if player's stones have different colors. if they are same return false
                    return false;

                gameGrid.Children.Remove(victim); //remove adversary

                playersLocation[yVictim, xVictim] = null; //freeup space after elimination in gamesGrid
                return true;
            }
            return false;
        }

        public Tuple<bool, bool> CheckAdversaryAlive()
        {
            bool white = false, black = false;

            foreach (var item in playersLocation)
            {
                if (item != null)
                {
                    switch (item.Name.Substring(0, 5))
                    {
                        case "white":
                            white = true;
                            break;

                        case "black":
                            black = true;
                            break;
                    }
                }

                if (black && white)
                    return Tuple.Create(true, false);
            }
            return Tuple.Create(false, white);
        }

        public void ResetGame()
        {
            gameGrid.Children.Clear();
            gameGrid.RowDefinitions.Clear();
            gameGrid.ColumnDefinitions.Clear();
        }
    }
}
