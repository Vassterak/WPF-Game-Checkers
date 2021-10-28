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
        private Checkers damaGame;
        private Random rnd = new Random();
        public FrameworkElement lastPlayerStonePosition;

        public MainWindow()
        {
            InitializeComponent();
            damaGame = new Checkers(gameGrid);
        }

        private void ButtonNewGame_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Reset and clear 
                damaGame.ResetGame();
                player1Label.Background = null;
                player2Label.Background = null;

                //Read new values from user input
                UserInputControl(int.Parse(textBoxWidth.Text), int.Parse(textBoxHeight.Text), int.Parse(textBoxNumStoneRows.Text));

                damaGame.CreateGameGrid();

                if (freeMovementCheckBox.IsChecked == true)
                {
                    damaGame.freeMovement = true;
                    MessageBox.Show("Volný pohyb je povolen, nyní nemůžeš vyřazovat proti-hráče hráče."); //Free movement is disabled. Now you are unable to eliminate the adversary.
                }

                else
                    damaGame.freeMovement = false;

                //Random selection figures colors for opening move
                if (rnd.Next(1, 6) > 3) 
                {
                    damaGame.whiteMove = true;
                    player1Label.Background = Brushes.Green;
                }

                else
                    player2Label.Background = Brushes.Green;

                damaGame.RenderCheckeredBackground();
                damaGame.RenderStones((Style)FindResource("playerStyle"));
            }

            catch (Exception error)
            {
                //Message that says: Only enter valid values!
                MessageBox.Show("Zadej pouze platné hodnoty! Chyba: " + error); 
            }
        }

        private void UserInputControl(int x, int y, int numberOfFiguresRows)
        {
            if (x > 3 && x < 100 && y > 3 && y < 100)
            {
                damaGame.XSize = x;
                damaGame.YSize = y;
            }

            else
                throw new Exception("Neplatné číselné hodnoty velikosti hracího pole.");

            if (numberOfFiguresRows + 4 < y && y > 0)
                damaGame.numOfStoneRows = numberOfFiguresRows;

            else
                throw new Exception("Neplatný počet řádků");

        }

        private void Ellipse_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                lastPlayerStonePosition = e.OriginalSource as FrameworkElement; //getting the object from MouseMove Event
                DragDrop.DoDragDrop(lastPlayerStonePosition, lastPlayerStonePosition, DragDropEffects.Move);

                //show attempts in UI
                damaGame.attempsForMove++; numOfTriedMoves.Content = damaGame.attempsForMove.ToString();
            }
        }

        private void gameGrid_Drop(object sender, DragEventArgs e) //after player removes finger from left click button.
        {
            //var newPosition = (UIElement)e.Source; //also working method, but cannot extract the "Name" from object
            var newPosition = e.OriginalSource as FrameworkElement; //get the element under mouse

            if (damaGame.validPosition[Grid.GetColumn(newPosition), Grid.GetRow(newPosition)]) //Checks if the player moved the stone to valid color of checkerboard
            {
                if (damaGame.playersLocation[Grid.GetRow(newPosition), Grid.GetColumn(newPosition)] == null &&
                   (lastPlayerStonePosition.Name.Substring(0, 5) == "black" ^ damaGame.whiteMove)) //check if there is no other player's stone (the destination space is empty) and if the right player is on the move
                {
                    if (damaGame.MoveOneWayOnly(lastPlayerStonePosition, newPosition) || damaGame.freeMovement) //check if player is moving in right direction
                    {
                        damaGame.playersLocation[Grid.GetRow(lastPlayerStonePosition), Grid.GetColumn(lastPlayerStonePosition)] = null;
                        Grid.SetColumn(lastPlayerStonePosition, Grid.GetColumn(newPosition));
                        Grid.SetRow(lastPlayerStonePosition, Grid.GetRow(newPosition));
                        damaGame.playersLocation[Grid.GetRow(newPosition), Grid.GetColumn(newPosition)] = (Ellipse)lastPlayerStonePosition;

                        damaGame.moves++; numOfValidMoves.Content = damaGame.moves.ToString();

                        damaGame.whiteMove = !damaGame.whiteMove;
                        if (damaGame.whiteMove)
                        {
                            setPlayerLabelColor(player1Label, true);
                            setPlayerLabelColor(player2Label, false);
                        }

                        else
                        {
                            setPlayerLabelColor(player1Label, false);
                            setPlayerLabelColor(player2Label, true);
                        }

                        var playersStatus = damaGame.CheckAdversaryAlive();
                        if (!playersStatus.Item1)
                            MessageBox.Show("Hra skončila! Vyhrál hráč s " + (playersStatus.Item2 ? "Bílými figurkami." : "Černými figurkami.")); //Player with white/black figures won.
                    }
                }
            }
        }

        private void setPlayerLabelColor(Label label, bool color)
        {
            label.Background = color ?  Brushes.Green : null;
        }

        private void debugButton_Click(object sender, RoutedEventArgs e) //Show all player's stones DEBUG purposes
        {
            string output = "";
            foreach (Ellipse item in damaGame.playersLocation)
            {
                if (item != null)
                {
                    output += item.Name.ToString();
                    output += "\n";
                }
            }

            MessageBox.Show(output);
        }

        private void gameInfoButton_Click(object sender, RoutedEventArgs e) //Not used. Yet
        {
            Settings settings = new Settings();
            settings.Show();
        }
    }
}
