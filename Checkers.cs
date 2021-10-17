using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;

namespace WPF_Game_Checkers
{
    class Checkers
    {
        //default values
        private int xSize = 8, ySize = 8;

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

        //Render checkered background
        public void RenderBackground(Canvas MyCanvas)
        {
            bool oddRectangle = true;

            for (int y = 0; y < YSize; y++)
            {
                for (int x = 0; x < XSize; x++)
                {
                    Rectangle rectangle = new Rectangle
                    {
                        Width = MyCanvas.ActualWidth / XSize,
                        Height = MyCanvas.ActualHeight / YSize,
                        Fill = oddRectangle ? Brushes.White : Brushes.SaddleBrown
                    };

                    MyCanvas.Children.Add(rectangle);
                    Canvas.SetLeft(rectangle, x * (MyCanvas.ActualWidth / XSize));
                    Canvas.SetTop(rectangle, y * (MyCanvas.ActualHeight / YSize));
                    //Creating the checkboard pattern
                    oddRectangle = !oddRectangle;
                    if (x == XSize - 1 && XSize % 2 == 0) //only true when x(rows) are even
                        oddRectangle = !oddRectangle;
                }
            }
            Grid.SetRowSpan(MyCanvas, XSize);
            Grid.SetColumnSpan(MyCanvas, YSize);
        }
    }
}
