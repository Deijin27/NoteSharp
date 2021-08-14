using System;

namespace Notes.ValueConverters
{
    public class AlphaConverter : ValueConverter<double, string>
    {
        protected override string Convert(double value)
        {
            return Math.Round(value, 2).ToString();
        }

        protected override double ConvertBack(string value)
        {
            return double.TryParse(value, out double i) ? i : 1.0;
        }
    }
}
