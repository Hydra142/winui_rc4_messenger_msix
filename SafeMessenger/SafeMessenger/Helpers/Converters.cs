using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeMessenge.Helpers
{
    internal class ArrayToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var result = value != null && value is IList val && val.Count > 0;
            return BoolToVisibility.GetVisibility(result, parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
    }

    internal class ObjectToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return BoolToVisibility.GetVisibility(value != null, parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();

    }

    public static class BoolToVisibility
    {
        public static Visibility GetVisibility(bool value, object getOpposite)
        {
            value = (string)getOpposite == "0" ? value : !value;
            return value ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
