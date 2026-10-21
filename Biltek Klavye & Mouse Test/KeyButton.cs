using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Biltek.KlavyeTest
{
    public class KeyButton : Control
    {
        static KeyButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(KeyButton),
                new FrameworkPropertyMetadata(typeof(KeyButton)));
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(KeyButton),
                new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty KeyEnabledProperty =
            DependencyProperty.Register(nameof(KeyEnabled), typeof(bool), typeof(KeyButton),
                new PropertyMetadata(false));

        public static readonly DependencyProperty KeyPressedProperty =
            DependencyProperty.Register(nameof(KeyPressed), typeof(bool), typeof(KeyButton),
                new PropertyMetadata(false));

        public static readonly DependencyProperty KeyCodeProperty =
            DependencyProperty.Register(nameof(KeyCode), typeof(Key), typeof(KeyButton),
                new PropertyMetadata(Key.None));

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), typeof(KeyButton),
                new PropertyMetadata(new CornerRadius(10)));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public bool KeyEnabled
        {
            get { return (bool)GetValue(KeyEnabledProperty); }
            set { SetValue(KeyEnabledProperty, value); }
        }

        public bool KeyPressed
        {
            get { return (bool)GetValue(KeyPressedProperty); }
            set { SetValue(KeyPressedProperty, value); }
        }

        public Key KeyCode
        {
            get { return (Key)GetValue(KeyCodeProperty); }
            set { SetValue(KeyCodeProperty, value); }
        }

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            if (ActualWidth > 0 && Math.Abs(ActualWidth - ActualHeight) < 1)
                CornerRadius = new CornerRadius(ActualHeight / 2);
            else
                CornerRadius = new CornerRadius(10);
        }
    }
}
