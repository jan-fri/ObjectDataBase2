using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BD2
{
    public static class TextBoxExtensions
    {
        public static DependencyProperty IsDirtyEnabledProperty = DependencyProperty.RegisterAttached("IsDirtyEnabled",
            typeof(bool), typeof(TextBoxExtensions), new PropertyMetadata(false, OnIsDirtyEnabledChanged));

        public static bool GetIsDirtyEnabled(TextBox target)
        {
            return (bool)target.GetValue(IsDirtyEnabledProperty);
        }

        public static void SetIsDirtyEnabled(TextBox target, bool value)
        {
            target.SetValue(IsDirtyEnabledProperty, value);
        }

        public static DependencyProperty ShowErrorTemplateProperty =
            DependencyProperty.RegisterAttached("ShowErrorTemplate",
                typeof(bool), typeof(TextBoxExtensions), new PropertyMetadata(false));

        public static bool GetShowErrorTemplate(TextBox target)
        {
            return (bool)target.GetValue(ShowErrorTemplateProperty);
        }

        public static void SetShowErrorTemplate(TextBox target, bool value)
        {
            target.SetValue(ShowErrorTemplateProperty, value);
        }

        private static void OnIsDirtyEnabledChanged(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs args)
        {
            var textBox = (TextBox)dependencyObject;
            if (textBox != null)
            {
                textBox.LostFocus += (s, e) =>
                {
                    if ((bool)textBox.GetValue(ShowErrorTemplateProperty) == false)
                    {
                        textBox.SetValue(ShowErrorTemplateProperty, true);
                    }
                };
            }
        }

        public static bool GetIsNumeric(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsNumericProperty);
        }

        public static void SetIsNumeric(DependencyObject obj, bool value)
        {
            obj.SetValue(IsNumericProperty, value);
        }

        public static readonly DependencyProperty IsNumericProperty = DependencyProperty.RegisterAttached("IsNumeric",
            typeof(bool), typeof(TextBoxExtensions), new PropertyMetadata(false, (s, e) =>
            {
                var targetTextbox = s as TextBox;
                if (targetTextbox != null)
                {
                    if ((bool)e.OldValue && !((bool)e.NewValue))
                    {
                        targetTextbox.PreviewTextInput -= targetTextbox_PreviewTextInput;
                    }
                    if ((bool)e.NewValue)
                    {
                        targetTextbox.PreviewTextInput += targetTextbox_PreviewTextInput;
                        targetTextbox.PreviewKeyDown += targetTextbox_PreviewKeyDown;
                    }
                }
            }));

        private static void targetTextbox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = e.Key == Key.Space;
        }

        private static void targetTextbox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Char newChar = e.Text[0];
            e.Handled = !Char.IsNumber(newChar);
        }

        public static bool GetIsDecimal(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsDecimalProperty);
        }

        public static void SetIsDecimal(DependencyObject obj, bool value)
        {
            obj.SetValue(IsDecimalProperty, value);
        }

        public static readonly DependencyProperty IsDecimalProperty = DependencyProperty.RegisterAttached("IsDecimal",
            typeof(bool), typeof(TextBoxExtensions), new PropertyMetadata(false, (s, e) =>
            {
                var targetTextbox = s as TextBox;
                if (targetTextbox != null)
                {
                    if ((bool)e.OldValue && !((bool)e.NewValue))
                    {
                        targetTextbox.PreviewTextInput -= IsDecimal_PreviewTextInput;
                    }
                    if ((bool)e.NewValue)
                    {
                        targetTextbox.PreviewTextInput += IsDecimal_PreviewTextInput;
                        targetTextbox.PreviewKeyDown += IsDecimal_PreviewKeyDown;
                    }
                }
            }));

        private static void IsDecimal_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = e.Key == Key.Space;
        }

        private static void IsDecimal_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            decimal val;
            var style = NumberStyles.AllowDecimalPoint;
            var textBox = sender as TextBox;
            var str = string.Concat(textBox.Text, e.Text[0]);
            e.Handled = !decimal.TryParse(str, style, CultureInfo.CurrentCulture, out val);
        }       
    }
}
