using Avalonia.Data.Converters;
using System;
using System.Globalization;
using cafeapp1.Models;

namespace cafeapp1.Converters
{
    public class ChangeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is User currentUser)
            {
                var dbUser = Service.GetContext().Users.Find(currentUser.Id);
                if (dbUser != null && currentUser.Equals(dbUser))
                {
                    return "Данные сохранены";
                }
                return "Есть несохраненные изменения";
            }
            return "Нет данных";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}