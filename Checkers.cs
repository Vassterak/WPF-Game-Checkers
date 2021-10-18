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
        private int xSize = 8, ySize = 8, numOfStoneRows = 2;
        public UIElement playerStone;

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
    }
}
