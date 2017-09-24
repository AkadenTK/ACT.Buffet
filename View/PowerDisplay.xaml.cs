using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Buffet
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class PowerDisplay : Window
    {
        public PowerDisplayVM VM
        { get { return (PowerDisplayVM)DataContext; } }

        public PowerDisplay(PowerDisplayVM vm)
        {
            InitializeComponent();
            DataContext = vm;
            this.MouseDown += Window_MouseDown;
            this.Closing += Window_Closing;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            if (VM.EnableClickThrough)
            {
                IntPtr hwnd = new WindowInteropHelper(this).Handle;
                int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
                SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT);
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private const int GWL_EXSTYLE = -20;

        public const int WS_EX_TRANSPARENT = 0x00000020;


        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    }
}
