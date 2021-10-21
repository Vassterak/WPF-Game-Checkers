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
    public partial class MainWindow : Window
    {
        private Checkers damaGame = new Checkers();
        private bool[,] validPositions;

        private int debugVar = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void CreateGameGrid()     //Crete grid for the game content.
        {
            validPositions = new bool[damaGame.XSize, damaGame.YSize]; //Initialize array

            for (int y = 0; y < damaGame.YSize; y++)
            {
                RowDefinition row = new RowDefinition();
                gameGrid.RowDefinitions.Add(row);
            }

            for (int x = 0; x < damaGame.XSize; x++)
            {
                ColumnDefinition colum = new ColumnDefinition();
                gameGrid.ColumnDefinitions.Add(colum);
            }
        }

        private void RenderCheckeredBackground()
        {
            bool oddRectangle = true;

            for (int y = 0; y < damaGame.YSize; y++)
            {
                for (int x = 0; x < damaGame.XSize; x++)
                {
                    validPositions[y, x] = oddRectangle;

                    Rectangle rectangle = new Rectangle {Fill = oddRectangle ? Brushes.LightGray : Brushes.SaddleBrown};
                    rectangle.SetValue(Grid.RowProperty, y);
                    rectangle.SetValue(Grid.ColumnProperty, x);
                    gameGrid.Children.Add(rectangle);

                    oddRectangle = OddChecker(oddRectangle, x); //Creating the checkboard pattern
                }
            }
        }

        private void RenderDebugText()
        {
            int numOfRects = 0;
            for (int y = 0; y < damaGame.YSize; y++)
            {
                for (int x = 0; x < damaGame.XSize; x++)
                {
                    Label laber = new Label
                    {
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        Content = numOfRects.ToString(),
                        FontSize = 20
                    };

                    laber.SetValue(Grid.RowProperty, y);
                    laber.SetValue(Grid.ColumnProperty, x);
                    gameGrid.Children.Add(laber);
                }
            }
        }

        private void ClearGameBoard()
        {
            gameGrid.Children.Clear();
            gameGrid.RowDefinitions.Clear();
            gameGrid.ColumnDefinitions.Clear();
            //players.Clear();
        }

        private void RenderStones(int numOfStonesRows)
        {
            bool blackPlane = false, player2render = false;

            for (int y = 0; y < damaGame.YSize; y++)
            {
                for (int x = 0; x < damaGame.XSize; x++)
                {
                    //Render stones only on brown spots
                    if (blackPlane)
                    {
                        if (y < numOfStonesRows) //render for player 1 (at top)
                        {
                            Ellipse playerStone = new Ellipse { Fill = Brushes.White, Margin = new Thickness(10)};
                            playerStone.SetValue(Grid.RowProperty, y);
                            playerStone.SetValue(Grid.ColumnProperty, x);
                            playerStone.Name = $"player1_{x}_{y}";
                            playerStone.Style = (Style)FindResource("playerStyle"); //Apply style with event handler
                            gameGrid.Children.Add(playerStone);
                            //players.Add(playerStone);
                        }

                        else //render for player 2 (at bottom)
                        {
                            if (!player2render)
                            {
                                y = damaGame.YSize - numOfStonesRows;
                                player2render = true;
                            }

                            Ellipse playerStone = new Ellipse { Fill = Brushes.Black, Margin = new Thickness(10) };
                            playerStone.SetValue(Grid.RowProperty, y);
                            playerStone.SetValue(Grid.ColumnProperty, x);
                            playerStone.Name = $"player2_{x}_{y}";
                            playerStone.Style = (Style)FindResource("playerStyle"); //Apply style with event handler
                            gameGrid.Children.Add(playerStone);
                           // players.Add(playerStone);
                        }
                    }

                    blackPlane = OddChecker(blackPlane, x); //Set if the checkboard plane is black
                }
            }
        }

        private void ButtonNewGame_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Clear everything
                ClearGameBoard();

                damaGame.XSize = int.Parse(textBoxWidth.Text);
                damaGame.YSize = int.Parse(textBoxHeight.Text);
                damaGame.NumOfStoneRows = int.Parse(textBoxNumStoneRows.Text);

                CreateGameGrid();
                RenderCheckeredBackground();
                RenderStones(damaGame.NumOfStoneRows);
            }

            catch (Exception)   //Message that says: Only enter valid values!
            {
                MessageBox.Show("Zadej pouze platné hodnoty!");
            }
        }

        private bool OddChecker(bool oddRectangle, int x)         //Return true only when last plate was false
        {
            oddRectangle = !oddRectangle;
            if (x == damaGame.XSize - 1 && damaGame.XSize % 2 == 0) //only true when x(rows) are even
                oddRectangle = !oddRectangle;

            return oddRectangle;
        }

        private void Ellipse_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var element = e.OriginalSource as FrameworkElement; //getting the object from MouseMove Event
                damaGame.playerStone = element;
                DragDrop.DoDragDrop(element, element, DragDropEffects.Move);
                debugVar++;
                debugVariable.Content = debugVar.ToString();
            }
        }

        private void gameGrid_Drop(object sender, DragEventArgs e)
        {
            var newPosition = (UIElement)e.Source;

            if (!validPositions[Grid.GetColumn(newPosition),Grid.GetRow(newPosition)])
            {
                Grid.SetColumn(damaGame.playerStone, Grid.GetColumn(newPosition));
                Grid.SetRow(damaGame.playerStone, Grid.GetRow(newPosition));
            }
        }

        private void ButtonTest_Click(object sender, RoutedEventArgs e)
        {
            string output = "";
            //foreach (var item in players)
            //    output = output + item.Name + "\n";


            foreach (var item in validPositions)
            {
                output = output + item + "\n";
            }
            output += output.Length.ToString();

            MessageBox.Show(output);
        }
    }
}
