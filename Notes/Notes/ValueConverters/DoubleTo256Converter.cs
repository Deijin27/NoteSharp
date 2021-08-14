
namespace Notes.ValueConverters
{
    public class DoubleTo256Converter : ValueConverter<double, string>
    {
        protected override string Convert(double value)
        {
            return ((byte)((double)value * 255)).ToString();
        }

        protected override double ConvertBack(string value)
        {
            return double.TryParse(value, out double i) ? i / 255 : 0;
        }
    }
}
