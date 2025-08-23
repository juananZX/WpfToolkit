using Espamatica.Core;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Espamatica.WpfToolkit
{
  public class MediaTimeConverter : IMultiValueConverter
  {
    /// <summary>
    /// Convierte un valor a <see cref="Espamatica.Core.MediaTime"/>.
    /// </summary>
    /// <param name="values">Valores a convertir. Ticks (long) y fracciones por segundo (int).</param>
    /// <param name="targetType">Tipo de la propiedad del destino de enlace.</param>
    /// <param name="parameter">Parámetro de convertidor que se va a usar.</param>
    /// <param name="culture">Referencia cultural que se va a usar en el convertidor.</param>
    /// <returns>Valor convertido. Si el método devuelve el valor Nothing, se utiliza el valor nulo válido.</returns>
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      long ticks = 0L;
      int fractionsPerSecond = MediaTime.DefaultFractionsPerSecond;

      if (values != null)
      {
        if (values.Length > 0)
        {
          if (!long.TryParse(System.Convert.ToString(values[0]), out ticks) && values[0] != DependencyProperty.UnsetValue)
            ticks = 0L;
        }

        if (values.Length > 1)
        {
          if (!int.TryParse(System.Convert.ToString(values[1]), out fractionsPerSecond) && values[1] != DependencyProperty.UnsetValue)
            fractionsPerSecond = MediaTime.DefaultFractionsPerSecond;

          if (fractionsPerSecond < 1)
            fractionsPerSecond = MediaTime.DefaultFractionsPerSecond;
        }
      }

      MediaTime time = new(ticks, fractionsPerSecond);

      return time;
    }

    /// <summary>
    /// Convierte un valor.
    /// </summary>
    /// <param name="value">Valor generado por el origen de enlace.</param>
    /// <param name="targetTypes">Tipo de la propiedad del destino de enlace.</param>
    /// <param name="parameter">Parámetro de convertidor que se va a usar.</param>
    /// <param name="culture">Referencia cultural que se va a usar en el convertidor.</param>
    /// <returns>Valor convertido. Si el método devuelve el valor Nothing, se utiliza el valor nulo válido.</returns>
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      long ticks = 0L;
      int fps = MediaTime.DefaultFractionsPerSecond;

      if (value is MediaTime time)
      {
        ticks = time.TotalTicks;
        fps = time.FractionsPerSeconds;
      }

      return [ticks, fps];
    }
  }
}
