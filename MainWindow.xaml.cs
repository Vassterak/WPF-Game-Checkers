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
        public MainWindow()
        {
            InitializeComponent();
        }

        private void CreteGridForBoard()
        {
            for (int y = 0; y < damaGame.YSize; y++)
            {
                RowDefinition row = new RowDefinition();
                gameFiledGrid.RowDefinitions.Add(row);
            }

            for (int x = 0; x < damaGame.XSize; x++)
            {
                ColumnDefinition colum = new ColumnDefinition();
                gameFiledGrid.ColumnDefinitions.Add(colum);
            }
        }
        private void RenderBackground()
        {
            for (int y = 0; y < damaGame.YSize; y++)
            {
                for (int x = 0; x < damaGame.XSize; x++)
                {

                }
            }
        }

        private void AddNumbersToGrid()
        {
            int numOfRects = 0;
            bool oddRectangle = true;

            for (int y = 0; y < damaGame.YSize; y++)
            {
                for (int x = 0; x < damaGame.XSize; x++)
                {
                    numOfRects++;

                    Rectangle rectangle = new Rectangle
                    {
                        Fill = oddRectangle ? Brushes.White : Brushes.SaddleBrown
                    };

                    Label laber = new Label
                    {
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        Content = numOfRects.ToString(),
                        FontSize = 20
                    };

                    rectangle.SetValue(Grid.RowProperty, y);
                    rectangle.SetValue(Grid.ColumnProperty, x);
                    gameFiledGrid.Children.Add(rectangle);

                    laber.SetValue(Grid.RowProperty, y);
                    laber.SetValue(Grid.ColumnProperty, x);
                    gameFiledGrid.Children.Add(laber);

                    //Creating the checkboard pattern
                    oddRectangle = !oddRectangle;
                    if (x == damaGame.XSize - 1 && damaGame.XSize % 2 == 0) //only true when x(rows) are even
                        oddRectangle = !oddRectangle;
                }
            }
        }

        private void ClearGameBoard()
        {
            gameFiledGrid.RowDefinitions.Clear();
            gameFiledGrid.ColumnDefinitions.Clear();
        }

        private void ButtonVykresli_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                damaGame.XSize = int.Parse(textBoxSirka.Text);
                damaGame.YSize = int.Parse(textBoxVyska.Text);
                CreteGridForBoard();
                AddNumbersToGrid();
            }

            catch (Exception)
            {
                //Message that says: Only enter valid values!
                MessageBox.Show("Zadej pouze platné hodnoty!");
            }
}

        private void ButtonVymaz_Click(object sender, RoutedEventArgs e)
        {
            ClearGameBoard();
        }
    }
}