using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace Biltek.KlavyeTest
{
    public partial class MainWindow : Window
    {
        private WinKeyBlocker winBlocker;
        public MainWindow()
        {
            InitializeComponent();
            winBlocker = new WinKeyBlocker { Active = true };
            winBlocker.WinKeyChanged += (key, down) => klavye.SetPressed(key, down);
            Closed += (s, e) => winBlocker.Dispose();
            PreviewKeyDown += (s, e) => klavye.SetPressed(RealKey(e), true);
            PreviewKeyUp += (s, e) => klavye.SetPressed(RealKey(e), false);
            AddHandler(PreviewMouseDownEvent,
                new MouseButtonEventHandler((s, e) => klavye.SetMousePressed(e.ChangedButton, true)), true);
            AddHandler(PreviewMouseUpEvent,
                new MouseButtonEventHandler((s, e) => klavye.SetMousePressed(e.ChangedButton, false)), true);
        }
        private static Key RealKey(KeyEventArgs e)
        {
            return e.Key == Key.System ? e.SystemKey : e.Key;
        }

    }
}
