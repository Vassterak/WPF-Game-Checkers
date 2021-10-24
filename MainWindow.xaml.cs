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
        private Checkers damaGame = new Checkers();
        private Random rnd = new Random();
        public FrameworkElement lastPlayerStonePosition;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonNewGame_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Reset and clear 
                damaGame.ResetGame(gameGrid);
                player1Label.Background = null;
                player2Label.Background = null;

                //Read new values from user input
                damaGame.XSize = int.Parse(textBoxWidth.Text);
                damaGame.YSize = int.Parse(textBoxHeight.Text);
                damaGame.numOfStoneRows = int.Parse(textBoxNumStoneRows.Text);

                damaGame.CreateGameGrid(gameGrid);

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

                damaGame.RenderCheckeredBackground(gameGrid);
                damaGame.RenderStones(gameGrid, (Style)FindResource("playerStyle"));
            }

            catch (Exception)
            {
                //Message that says: Only enter valid values!
                MessageBox.Show("Zadej pouze platné hodnoty!"); 
            }
        }

        private void Ellipse_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                lastPlayerStonePosition = e.OriginalSource as FrameworkElement; //getting the object from MouseMove Event
                DragDrop.DoDragDrop(lastPlayerStonePosition, lastPlayerStonePosition, DragDropEffects.Move);

                //show attems in UI
                damaGame.attempsForMove++;
                debugVariable.Content = damaGame.attempsForMove.ToString();
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

                    if (damaGame.MoveOneWayOnly(lastPlayerStonePosition, newPosition, gameGrid) || damaGame.freeMovement) //check if player is moving in right direction
                    {
                        damaGame.playersLocation[Grid.GetRow(lastPlayerStonePosition), Grid.GetColumn(lastPlayerStonePosition)] = null;
                        Grid.SetColumn(lastPlayerStonePosition, Grid.GetColumn(newPosition));
                        Grid.SetRow(lastPlayerStonePosition, Grid.GetRow(newPosition));

                        damaGame.playersLocation[Grid.GetRow(newPosition), Grid.GetColumn(newPosition)] = (Ellipse)lastPlayerStonePosition;

                        damaGame.moves++;
                        debugVariable2.Content = damaGame.moves.ToString();

                        var playersStatus = damaGame.CheckAdversaryAlive();
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

        private void ButtonTest_Click(object sender, RoutedEventArgs e) //Show all player's stones DEBUG purposes
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
    }
}
