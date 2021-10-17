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
            damaGame.RenderBackground(gameCanvas);
        }

        private void ButtonVykresli_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                damaGame.XSize = int.Parse(textBoxSirka.Text);
                damaGame.YSize = int.Parse(textBoxVyska.Text);

                ClearGameBoard();

                damaGame.RenderBackground(gameCanvas);
                CreteGridForBoard();
            }
            catch (Exception)
            {
                //Message that says: Only enter valid values!
                MessageBox.Show("Zadej pouze platné hodnoty!");
            }
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

        private void AddNumberToGrid()
        {
            Label laber = new Label
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Content = x.ToString(),
                FontSize = 20
            };
            laber.SetValue(Grid.RowProperty, x);
            laber.SetValue(Grid.ColumnProperty, 0);
            gameFiledGrid.Children.Add(laber);
        }

        private void ClearGameBoard()
        {
            gameCanvas.Children.Clear();
            gameFiledGrid.RowDefinitions.Clear();
            gameFiledGrid.ColumnDefinitions.Clear();
        }
    }
}
