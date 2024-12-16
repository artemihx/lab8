using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using System;
using System.Globalization;
using Avalonia.Platform;
using Bitmap = Avalonia.Media.Imaging.Bitmap;

namespace cafeapp1.Converters
{
    public class AvatarConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()) || value.ToString().Equals("[null]", StringComparison.OrdinalIgnoreCase))
            {
                return new Bitmap(AssetLoader.Open(new Uri("avares://cafeapp1/Assets/null-avatar.jpeg")));
            }
            return new Bitmap(AssetLoader.Open(new Uri("avares://cafeapp1/Assets/users-photo/" + value.ToString())));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}