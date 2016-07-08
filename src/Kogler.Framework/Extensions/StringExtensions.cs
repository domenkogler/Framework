using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace Kogler.Framework
{
    public static class StringExtensions
    {
        #region << Public >>
        
        public static Color ToColor(this string color)
        {
            if (color[0] == '#') color = color.Substring(1);
            if (color.Length != 6 && color.Length != 8) throw new ArgumentOutOfRangeException("color");
            int start = 0;
            byte a;
            if (color.Length == 8)
            {
                a = byte.Parse(color.Substring(0, 2), NumberStyles.HexNumber);
                start = 2;
            }
            else a = 255;
            byte r = byte.Parse(color.Substring(start, 2), NumberStyles.HexNumber);
            byte g = byte.Parse(color.Substring(start + 2, 2), NumberStyles.HexNumber);
            byte b = byte.Parse(color.Substring(start + 4, 2), NumberStyles.HexNumber);
            return new Color {A = a, R = r, B = b, G = g};
        }
        
        public static ContentControl ToSearchContent(this string searchedString, string search,
                                                     Style defaultTextBlockStyle = null,
                                                     Style matchTextBlockStyle = null)
        {
            var cc = new ContentControl();
            if (string.IsNullOrEmpty(searchedString) || string.IsNullOrEmpty(search))
            {
                cc.Content = GetTextBlock(searchedString, defaultTextBlockStyle);
                return cc;
            }

            if (matchTextBlockStyle == null)
            {
                matchTextBlockStyle = new Style {TargetType = typeof (TextBlock)};
                matchTextBlockStyle.Setters.Add(new Setter(TextBlock.FontWeightProperty, FontWeights.Bold));
            }

            var sp = new StackPanel {Orientation = Orientation.Horizontal};
            int srcIndex = 0;
            do
            {
                int srcNewIndex = searchedString.ToLowerInvariant().IndexOf(search.ToLowerInvariant(), srcIndex, StringComparison.Ordinal);
                if (srcNewIndex != -1)
                {
                    sp.Children.Add(GetTextBlock(searchedString.Substring(srcIndex, srcNewIndex - srcIndex), defaultTextBlockStyle));
                    sp.Children.Add(GetTextBlock(searchedString.Substring(srcNewIndex, search.Length), matchTextBlockStyle));
                    srcIndex = srcNewIndex + search.Length;
                }
                else
                {
                    sp.Children.Add(GetTextBlock(searchedString.Substring(srcIndex), defaultTextBlockStyle));
                    srcIndex = -1;
                }
            } while (srcIndex != -1);
            cc.Content = sp;
            return cc;
        }

        private static TextBlock GetTextBlock(string text, Style style)
        {
            return new TextBlock {Text = text, Style = style};
        }

        public static TextBlock ToTextBlockText(this string me)
        {
            if (me == null) me = string.Empty;
            return new TextBlock {Text = me.Replace(@"\n\r", Environment.NewLine), TextWrapping = TextWrapping.Wrap};
        }
        
        #endregion
    }
}