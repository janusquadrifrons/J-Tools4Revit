using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Collections;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace J_Tools
{
    /// <summary>
    /// Interaction logic for FormWindow.xaml
    /// </summary>
    public partial class FormWindow : Window
    {
        public FormWindow(IEnumerable collection)
        {
            InitializeComponent();
            Box.ItemsSource = collection;
        }
    }
}
