using System;
using System.Globalization;
using System.Windows.Data;

namespace LostArkAuctionHelper.Helpers
{
  internal class PartyOptionToBorderThicknessConverter : IValueConverter
  {
    public int ActiveValue { get; set; }
    public int InactiveValue { get; set; }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var intVal = (int)value;
      var intParam = int.Parse(parameter.ToString());

      return intVal == intParam ? ActiveValue : InactiveValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
