using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;

namespace Biltek.KlavyeTest
{
    public partial class KeyboardView : UserControl
    {
        private readonly Dictionary<Key, List<KeyButton>> map = new Dictionary<Key, List<KeyButton>>();
        private readonly List<KeyButton> allKeys = new List<KeyButton>();

        public KeyboardView()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                BuildMap();
                ComponentDispatcher.ThreadPreprocessMessage -= OnMessage;
                ComponentDispatcher.ThreadPreprocessMessage += OnMessage;
            };
            Unloaded += (s, e) => ComponentDispatcher.ThreadPreprocessMessage -= OnMessage;
        }

        private void BuildMap()
        {
            map.Clear();
            allKeys.Clear();
            Collect(this);
        }

        private void Collect(DependencyObject parent)
        {
            foreach (object child in LogicalTreeHelper.GetChildren(parent))
            {
                var key = child as KeyButton;
                if (key != null)
                {
                    allKeys.Add(key);
                    if (key.KeyCode != Key.None)
                    {
                        List<KeyButton> list;
                        if (!map.TryGetValue(key.KeyCode, out list))
                            map[key.KeyCode] = list = new List<KeyButton>();
                        list.Add(key);
                    }
                }
                var dep = child as DependencyObject;
                if (dep != null) Collect(dep);
            }
        }
        public void SetPressed(Key key, bool pressed)
        {
            List<KeyButton> list;
            if (map.TryGetValue(key, out list))
            {
                foreach (var k in list)
                {
                    k.KeyPressed = pressed;
                    if (!pressed)
                        k.KeyEnabled = true;
                }
            }
        }
        public void SetMousePressed(MouseButton button, bool pressed)
        {
            KeyButton k = null;
            switch (button)
            {
                case MouseButton.Left: k = BtnSolTik; break;
                case MouseButton.Middle: k = BtnOrtaTik; break;
                case MouseButton.Right: k = BtnSagTik; break;
            }
            if (k == null) return;

            k.KeyPressed = pressed;
            if (!pressed) k.KeyEnabled = true;
        }
        private void OnMessage(ref MSG msg, ref bool handled)
        {
            const int WM_KEYDOWN = 0x100, WM_KEYUP = 0x101, WM_SYSKEYDOWN = 0x104, WM_SYSKEYUP = 0x105;
            const int VK_RETURN = 0x0D;

            if ((int)msg.wParam != VK_RETURN) return;

            bool down = msg.message == WM_KEYDOWN || msg.message == WM_SYSKEYDOWN;
            bool up = msg.message == WM_KEYUP || msg.message == WM_SYSKEYUP;
            if (!down && !up) return;

            bool extended = ((long)msg.lParam & 0x01000000) != 0;   // numpad Enter
            KeyButton k = extended ? BtnNumEnter : BtnEnter;

            k.KeyPressed = down;
            if (up) k.KeyEnabled = true;
        }
    }
}
