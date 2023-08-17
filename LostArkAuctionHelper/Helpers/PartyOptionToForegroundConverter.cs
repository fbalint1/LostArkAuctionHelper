using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace LostArkAuctionHelper.Helpers
{
  internal class PartyOptionToForegroundConverter : IValueConverter
  {
    private readonly static SolidColorBrush DARK_PURPLE_BRUSH = new SolidColorBrush(new Color() { A = 255, R = 72, G = 66, B = 109 });
    private readonly static SolidColorBrush GOLD_BRUSH = new SolidColorBrush(new Color() { A = 255, R = 240, G = 195, B = 142 });

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var intVal = (int)value;
      var intParam = int.Parse(parameter.ToString());

      return intVal == intParam ? DARK_PURPLE_BRUSH : GOLD_BRUSH;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
