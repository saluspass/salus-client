using System;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace Salus
{
    class StringToHiddenStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == typeof(string))
            {
                StringBuilder builder = new StringBuilder();
                foreach (char c in (value as string))
                {
                    builder.Append("*");
                }
                return builder.ToString();
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
