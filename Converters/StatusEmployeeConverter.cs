using Avalonia.Data.Converters;
using System;
using System.Globalization;
using cafeapp1.Models;

namespace cafeapp1.Converters
{
    public class StatusEmployeeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is User user)
            {
                if (user.Status == true)
                {
                    return "В работе";
                }
                return "Уволен";
            }

            return "Ошибка";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}