using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace LostArkAuctionHelper.Helpers
{
  internal class PartyOptionBackgroundConverter : IValueConverter
  {
    public SolidColorBrush ActiveColor { get; set; }
    public SolidColorBrush InactiveColor { get; set; }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var intVal = (int)value;
      var intParam = int.Parse(parameter.ToString());

      return intVal == intParam ? ActiveColor : InactiveColor;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
