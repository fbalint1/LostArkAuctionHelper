using System;
using System.Collections.Generic;
using System.Linq;

namespace LostArkAuctionHelper.Helpers
{
  internal static class NumericExtensions
  {
    internal static bool TryParseInt(this string toParse_, out int value_)
    {
      return int.TryParse(toParse_, out value_);
    }
  }
}
