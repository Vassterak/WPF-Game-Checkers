﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPF_Game_Checkers
{
    class Checkers
    {
        //default values
        private int xSize = 8, ySize = 8, numOfStoneRows = 2;
        public bool[,] validPosition;
        public FrameworkElement playerStone;
        public FrameworkElement lastPlayerStonePosition;
        public Ellipse[,] playersLocation;
        public bool freeMovement = false, whiteMove = false;
        public int attempsForMove = 0, moves = 0;

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

        public void CreateGameGrid(Grid gameGrid) //Crete grid for the game content.
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

        public void RenderCheckeredBackground(Grid gameGrid)
        {
            bool oddRectangle = true;

            for (int y = 0; y < YSize; y++)
            {
                for (int x = 0; x < XSize; x++)
                {
                    validPosition[y, x] = !oddRectangle;

                    Rectangle rectangle = new Rectangle { Fill = oddRectangle ? Brushes.LightGray : Brushes.SaddleBrown };
                    rectangle.SetValue(Grid.RowProperty, y);
                    rectangle.SetValue(Grid.ColumnProperty, x);
                    gameGrid.Children.Add(rectangle);

                    oddRectangle = OddChecker(oddRectangle, x); //Creating the checkboard pattern
                }
            }
        }

        public void RenderStones(int numOfStonesRows, Grid gameGrid, Style stoneStyle)
        {
            bool blackPlane = false, player2render = false;

            for (int y = 0; y < YSize; y++)
            {
                for (int x = 0; x < XSize; x++)
                {
                    //Render stones only on brown spots
                    if (blackPlane)
                    {
                        if (y < numOfStonesRows) //render for player 1 (at top)
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
                                y = YSize - numOfStonesRows;
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

        public bool OddChecker(bool oddRectangle, int x) //Return true only when last plate was false
        {
            oddRectangle = !oddRectangle;
            if (x == XSize - 1 && XSize % 2 == 0) //only true when x(rows) are even
                oddRectangle = !oddRectangle;

            return oddRectangle;
        }

        public bool MoveOneWayOnly(FrameworkElement lastPostion, FrameworkElement currentPosition, Grid gameGrid)
        {
            int yLast = 0, yNow = 0;

            yLast = Grid.GetRow(lastPostion);
            yNow = Grid.GetRow(currentPosition);

            if (lastPostion.Name.Substring(0, 5) == "white")
            {
                if ((yNow - yLast) == 1)
                    return true;

                else if ((yNow - yLast) == 2)
                    return CheckKill(lastPostion, currentPosition, gameGrid);
            }

            else
            {
                if ((yNow - yLast) == -1)
                    return true;

                else if ((yNow - yLast) == -2)
                    return CheckKill(lastPostion, currentPosition, gameGrid);
            }

            return false;
        }

        public bool CheckKill(FrameworkElement lastPosition, FrameworkElement currentPosition, Grid gameGrid)
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

        public void ResetGame(Grid gameGrid)
        {
            gameGrid.Children.Clear();
            gameGrid.RowDefinitions.Clear();
            gameGrid.ColumnDefinitions.Clear();
        }
    }
}
