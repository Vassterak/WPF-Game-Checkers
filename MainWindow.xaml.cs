using System;
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
        private Random rnd = new Random();
        private Checkers damaGame = new Checkers();
        private bool[,] validPosition;
        private Ellipse[,] playersLocation;
        private bool freeMovement = false, whiteMove = false;
        private int attempsForMove = 0, moves = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void CreateGameGrid() //Crete grid for the game content.
        {
            //Initialize arrays
            validPosition = new bool[damaGame.XSize, damaGame.YSize]; 
            playersLocation = new Ellipse[damaGame.XSize, damaGame.YSize];

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

            if (freeMovementCheckBox.IsChecked == true)
            {
                freeMovement = true;
                MessageBox.Show("Volný pohyb je povolen, nyní nemůžeš vyřazovat proti-hráče hráče."); //Free movement is disabled. Now you are unable to eliminate the adversary.
            }
            else
                freeMovement = false;

            if (rnd.Next(1, 6) > 3) //Random selection figures colors for opening move
            {
                whiteMove = true;
                player1Label.Background = Brushes.Green;
            }
            else
                player2Label.Background = Brushes.Green;
        }

        private void RenderCheckeredBackground()
        {
            bool oddRectangle = true;

            for (int y = 0; y < damaGame.YSize; y++)
            {
                for (int x = 0; x < damaGame.XSize; x++)
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

        private void ResetGame()
        {
            gameGrid.Children.Clear();
            gameGrid.RowDefinitions.Clear();
            gameGrid.ColumnDefinitions.Clear();
            player1Label.Background = null;
            player2Label.Background = null;
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
                            Ellipse newPlayerStone = new Ellipse { Fill = Brushes.White, Margin = new Thickness(10) };
                            newPlayerStone.SetValue(Grid.RowProperty, y);
                            newPlayerStone.SetValue(Grid.ColumnProperty, x);
                            newPlayerStone.Name = $"white_{x}_{y}";
                            newPlayerStone.Style = (Style)FindResource("playerStyle"); //Apply style with event handler

                            gameGrid.Children.Add(newPlayerStone); //Adding player's stone to be rendered in gameGrid
                            playersLocation[y, x] = newPlayerStone;
                        }

                        else //render for player 2 (at bottom)
                        {
                            if (!player2render)
                            {
                                y = damaGame.YSize - numOfStonesRows;
                                player2render = true;
                            }

                            Ellipse newPlayerStone = new Ellipse { Fill = Brushes.Black, Margin = new Thickness(10) };
                            newPlayerStone.SetValue(Grid.RowProperty, y);
                            newPlayerStone.SetValue(Grid.ColumnProperty, x);
                            newPlayerStone.Name = $"black_{x}_{y}";
                            newPlayerStone.Style = (Style)FindResource("playerStyle"); //Apply style with event handler

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

        private void ButtonNewGame_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Clear everything
                ResetGame();

                damaGame.XSize = int.Parse(textBoxWidth.Text);
                damaGame.YSize = int.Parse(textBoxHeight.Text);
                damaGame.NumOfStoneRows = int.Parse(textBoxNumStoneRows.Text);

                CreateGameGrid();
                RenderCheckeredBackground();
                RenderStones(damaGame.NumOfStoneRows);
            }

            catch (Exception)   
            {
                MessageBox.Show("Zadej pouze platné hodnoty!"); //Message that says: Only enter valid values!
            }
        }

        private bool OddChecker(bool oddRectangle, int x) //Return true only when last plate was false
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
                damaGame.lastPlayerStonePosition = element;
                DragDrop.DoDragDrop(element, element, DragDropEffects.Move);

                attempsForMove++;
                debugVariable.Content = attempsForMove.ToString();
            }
        }

        private void gameGrid_Drop(object sender, DragEventArgs e)
        {
            //var newPosition = (UIElement)e.Source; //also working method, but cannot extract the "Name" from object
            var newPosition = e.OriginalSource as FrameworkElement; //get the element under mouse

            if (validPosition[Grid.GetColumn(newPosition), Grid.GetRow(newPosition)] && (damaGame.lastPlayerStonePosition.Name.Substring(0, 5) == "black" ^ whiteMove) ) //Checks if the player moved the stone to valid color of checkerboard and if the player is allow to make move
            {
                whiteMove = !whiteMove;
                if (whiteMove)
                {
                    setPlayerLabelColor(player1Label, true);
                    setPlayerLabelColor(player2Label, false);
                }
                else
                {
                    setPlayerLabelColor(player1Label, false);
                    setPlayerLabelColor(player2Label, true);
                }

                if (playersLocation[Grid.GetRow(newPosition), Grid.GetColumn(newPosition)] == null) //check if there is no other player's stone (the destination space is empty)
                {
                    if (MoveOneWayOnly(damaGame.lastPlayerStonePosition, newPosition) || freeMovement) //check if player is moving in right direction
                    {
                        playersLocation[Grid.GetRow(damaGame.lastPlayerStonePosition), Grid.GetColumn(damaGame.lastPlayerStonePosition)] = null;
                        Grid.SetColumn(damaGame.playerStone, Grid.GetColumn(newPosition));
                        Grid.SetRow(damaGame.playerStone, Grid.GetRow(newPosition));

                        playersLocation[Grid.GetRow(newPosition), Grid.GetColumn(newPosition)] = (Ellipse)damaGame.playerStone;
                        moves++;
                        debugVariable2.Content = moves.ToString();

                        var playersStatus = CheckAdversaryAlive();
                        if (!playersStatus.Item1)
                            MessageBox.Show("Hra skončila! Vyhrál hráč s " + (playersStatus.Item2 ? "Bílými figurkami." : "Černými figurkami.")); //Player with white/black figures won.
                    }
                }
            }
        }
        private void setPlayerLabelColor(Label label, bool color)
        {
            if (color)
                label.Background = Brushes.Green;
            else
                label.Background = null;
        }

        private bool MoveOneWayOnly(FrameworkElement lastPostion, FrameworkElement currentPosition)
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

            if (playersLocation[yVictim, xVictim] != null)
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

        private Tuple<bool, bool> CheckAdversaryAlive()
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

        private void ButtonTest_Click(object sender, RoutedEventArgs e)
        {
            string output = "";
            foreach (Ellipse item in playersLocation)
            {
                if (item != null)
                {
                    output += item.Name.ToString();
                    output += "\n";
                }
            }

            MessageBox.Show(output);
        }
    }
}
