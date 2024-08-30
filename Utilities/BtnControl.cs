using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace Boardgame.Utilities
{
    public class BtnControl : RadioButton
    {
        static BtnControl()
            {
                DefaultStyleKeyProperty.OverrideMetadata(typeof(BtnControl),new FrameworkPropertyMetadata(typeof(BtnControl)));
            }
    }
}
