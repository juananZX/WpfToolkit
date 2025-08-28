using System.Globalization;

namespace Espamatica.Core
{
  /// <summary>
  /// Structure for multimedia time management.
  /// Estructura para manejo de tiempos multimedia.
  /// </summary>
  /// <see cref="https://espamatica.com/mediatime/"/>
  public struct MediaTime
  {
    #region Public consts
    /// <summary>
    /// Default fractions per second.
    /// Fracciones por defecto por segundo.
    /// </summary>
    public const int DefaultFractionsPerSecond = 1000;

    /// <summary>
    /// Number of ticks per millisecond.
    /// Número de ticks por milisegundo.
    /// </summary>
    public const long TicksMillisecond = 10000;

    /// <summary>
    /// Number of milliseconds per second.
    /// Número de milisegundos por segundo.
    /// </summary>
    public const long MillisecondsSecond = 1000;

    /// <summary>
    /// Number of seconds per minute.
    /// Número de segundos por minuto.
    /// </summary>
    public const long SecondsMinute = 60;

    /// <summary>
    /// Number of minutes per hour.
    /// Número de minutos por hora.
    /// </summary>
    public const long MinutesHour = 60;

    /// <summary>
    /// Number of hours per day.
    /// Número de horas por día.
    /// </summary>
    public const long HoursDay = 24;

    /// <summary>
    /// Number of ticks per second.
    /// Número de ticks por segundo.
    /// </summary>
    public const long TicksSecond = TicksMillisecond * MillisecondsSecond;

    /// <summary>
    /// Number of ticks per minute.
    /// Número de ticks por minuto.
    /// </summary>
    public const long TicksMinute = TicksSecond * SecondsMinute;

    /// <summary>
    /// Number of ticks per hour.
    /// Número de ticks por hora.
    /// </summary>
    public const long TicksHour = TicksMinute * MinutesHour;

    /// <summary>
    /// Number of ticks per day.
    /// Número de ticks por día.
    /// </summary>
    public const long TicksDay = TicksHour * HoursDay;
    #endregion

    #region Private fields
    /// <summary>
    /// Control field for the Days property.
    /// Campo de control para la propiedad Days.
    /// </summary>
    private int days;

    /// <summary>
    /// Control field for the DigitPerFraction property.
    /// Campo de control para la propiedad DigitPerFraction.
    /// </summary>
    private int digitPerFraction;

    /// <summary>
    /// Control field for the Fractions property.
    /// Campo de control para la propiedad Fractions.
    /// </summary>
    private double fractions;

    /// <summary>
    /// Control field for the FractionsPerSecond property.
    /// Campo de control para la propiedad FractionsPerSecond.
    /// </summary>
    private int fractionsPerSecond;

    /// <summary>
    /// Control field for the Hours property.
    /// Campo de control para la propiedad Hours.
    /// </summary>
    private int hours;

    /// <summary>
    /// Control field for the Milliseconds property.
    /// Campo de control para la propiedad Milliseconds.
    /// </summary>
    private int milliseconds;

    /// <summary>
    /// Control field for the MillisecondsPerFraction property.
    /// Campo de control para la propiedad MillisecondsPerFraction.
    /// </summary>
    private double millisecondsPerFraction;

    /// <summary>
    /// Control field for the Minutes property.
    /// Campo de control para la propiedad Minutes.
    /// </summary>
    private int minutes;

    /// <summary>
    /// Control field for the RoundMode property.
    /// Campo de control para la propiedad RoundMode.
    /// </summary>
    private FractionRoundMode roundMode;

    /// <summary>
    /// Control field for the Seconds property.
    /// Campo de control para la propiedad Seconds.
    /// </summary>
    private int seconds;

    /// <summary>
    /// Control field for the Ticks property.
    /// Campo de control para la propiedad Ticks.
    /// </summary>
    private int ticks;

    /// <summary>
    /// Control field for the Ticks property.
    /// Campo de control para la propiedad Ticks.
    /// </summary>
    private long totalTicks;
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the MediaTime structure.
    /// Inicializa una nueva instancia de la estructura MediaTime.
    /// </summary>
    /// <param name="time">
    /// Time in ticks.
    /// Tiempo en ticks.
    /// </param>
    /// <param name="fractionsPerSecond">
    /// Fractions per second (between 1 and 1000).
    /// Fracciones por segundo (entre 1 y 1000).
    /// </param>
    public MediaTime(long time, int fractionsPerSecond, FractionRoundMode fractionRoundMode = FractionRoundMode.Truncate)
    {
      if (fractionsPerSecond < 1 || fractionsPerSecond > MillisecondsSecond)
        throw new ArgumentOutOfRangeException(nameof(fractionsPerSecond), fractionsPerSecond, string.Format("to {0} from {1}", 1, MillisecondsSecond));

      days = 0;
      digitPerFraction = 0;
      fractions = 0;
      this.fractionsPerSecond = fractionsPerSecond;
      hours = 0;
      milliseconds = 0;
      millisecondsPerFraction = 0.0;
      minutes = 0;
      roundMode = fractionRoundMode;
      seconds = 0;
      ticks = 0;
      totalTicks = time;

      RefreshFractionPerSeconds();
      RefreshMediaTime();
    }
    #endregion

    #region Public properties
    /// <summary>
    /// Gets or sets the days.
    /// Obtiene o establece los días.
    /// </summary>
    public int Days
    {
      readonly get => days;
      set
      {
        if (value != days)
        {
          days = value;
          RefreshTime();
          RefreshMediaTime();
        }
      }
    }

    /// <summary>
    /// Gets the number of digits used to represent fractions.
    /// Obtiene el número de dígitos usados para representar las fracciones.
    /// </summary>
    public int DigitPerFraction
    {
      readonly get => digitPerFraction;
      private set => digitPerFraction = value;
    }

    /// <summary>
    /// Gets or sets fractions.
    /// Obtiene o establece las fracciones.
    /// </summary>
    public double Fractions
    {
      readonly get => fractions;
      set
      {
        if (double.IsNaN(value)) value = 0;

        if (value != fractions)
        {
          fractions = value;
          Milliseconds = (int)Math.Truncate(value * MillisecondsPerFraction);
        }
      }
    }

    /// <summary>
    /// Gets or sets the number of fractions per second.
    /// Obtiene o establece el número de fracciones por segundo.
    /// </summary>
    public int FractionsPerSeconds
    {
      readonly get => fractionsPerSecond;
      set
      {
        if (value != fractionsPerSecond)
        {
          if (value < 1 || value > (int)MillisecondsSecond)
            value = 1000;

          fractionsPerSecond = value;
          RefreshFractionPerSeconds();
          RefreshMediaTime();
        }
      }
    }

    /// <summary>
    /// Gets or sets the time.
    /// Obtiene o establece las horas.
    /// </summary>
    public int Hours
    {
      readonly get => hours;
      set
      {
        if (value != hours)
        {
          hours = value;
          RefreshTime();
          RefreshMediaTime();
        }
      }
    }

    /// <summary>
    /// Gets or sets the milliseconds.
    /// Obtiene o establece los milisegundos.
    /// </summary>
    public int Milliseconds
    {
      readonly get => milliseconds;
      set
      {
        if (value != milliseconds)
        {
          milliseconds = value;
          RefreshTime();
          RefreshMediaTime();
        }
      }
    }

    /// <summary>
    /// Gets the number of milliseconds per fraction.
    /// Obiene o establece los milisegundos por fracción.
    /// </summary>
    public double MillisecondsPerFraction
    {
      readonly get => millisecondsPerFraction;
      private set => millisecondsPerFraction = value;
    }

    /// <summary>
    /// Gets or sets the minutes.
    /// Obtiene o establece los minutos.
    /// </summary>
    public int Minutes
    {
      readonly get => minutes;
      set
      {
        if (value != minutes)
        {
          minutes = value;
          RefreshTime();
          RefreshMediaTime();
        }
      }
    }

    /// <summary>
    /// Gets or sets the mode in which fractions are rounded when represented in text.
    /// Obtiene o establece el modo en el que se redondean las fracciones a la hora de representarlas en texto.
    /// </summary>
    public FractionRoundMode RoundMode
    {
      readonly get => roundMode;
      set => roundMode = value;
    }

    /// <summary>
    /// Gets or sets the seconds.
    /// Obtiene o establece los segundos.
    /// </summary>
    public int Seconds
    {
      readonly get => seconds;
      set
      {
        if (value != seconds)
        {
          seconds = value;
          RefreshTime();
          RefreshMediaTime();
        }
      }
    }

    /// <summary>
    /// Gets or sets the ticks.
    /// Obtiene o establece los ticks.
    /// </summary>
    public int Ticks
    {
      readonly get => ticks;
      set
      {
        if (value != ticks)
        {
          ticks = value;
          RefreshTime();
          RefreshMediaTime();
        }
      }
    }

    /// <summary>
    /// Gets the total fractions.
    /// Obtiene las fracciones totales.
    /// </summary>
    public readonly long TotalFractions => TotalSeconds * FractionsPerSeconds;

    /// <summary>
    /// Gets the total hours.
    /// Obtiene las horas totales.
    /// </summary>
    public readonly long TotalHours => totalTicks / TicksHour;

    /// <summary>
    /// Gets the total milliseconds.
    /// Obtiene los milisegundos totales.
    /// </summary>
    public readonly long TotalMilliseconds => totalTicks / TicksMillisecond;

    /// <summary>
    /// Gets the total minutes.
    /// Obtiene los minutos totales.
    /// </summary>
    public readonly long TotalMinutes => totalTicks / TicksMinute;

    /// <summary>
    /// Gets the total seconds.
    /// Obtiene los segundos totales.
    /// </summary>
    public readonly long TotalSeconds => totalTicks / TicksSecond;

    /// <summary>
    /// Gets or sets the time in ticks.
    /// Obtiene o establece el tiempo en ticks.
    /// </summary>
    public long TotalTicks
    {
      readonly get => totalTicks;
      set
      {
        if (value != totalTicks)
        {
          totalTicks = value;
          RefreshMediaTime();
        }
      }
    }
    #endregion

    #region Operators
    /// <summary>
    /// Compares two objects to determine whether the first specified object is less than the second specified object.
    /// Compara dos objetos para determinar si el primer objeto especificado es menor que el segundo objeto especificado.
    /// </summary>
    /// <param name="obj1">
    /// First specified object.
    /// Primer objeto especificado.
    /// </param>
    /// <param name="obj2">
    /// Second specified object.
    /// Segundo objeto especificado.
    /// </param>
    /// 
    /// <returns>
    /// If the first specified object is less than the second specified object, true; otherwise, false.
    /// Si el primer objeto especificado es menor que el segundo objeto especificado true, en caso contrario false.
    /// </returns>
    public static bool operator <(MediaTime obj1, MediaTime obj2) => obj1.TotalTicks < obj2.TotalTicks;

    /// <summary>
    /// Compares two objects to determine whether the first specified object is less than or equal to the second specified object.
    /// Compara dos objetos para determinar si el primer objeto especificado es menor o igual que el segundo objeto especificado.
    /// </summary>
    /// <param name="obj1">
    /// First specified object.
    /// Primer objeto especificado.
    /// </param>
    /// <param name="obj2">
    /// Second specified object.
    /// Segundo objeto especificado.
    /// </param>
    /// <returns>
    /// If the first specified object is less than or equal to the second specified object, true; otherwise, false.
    /// Si el primer objeto especificado es menor o igual que el segundo objeto especificado true, en caso contrario false.
    /// </returns>
    public static bool operator <=(MediaTime obj1, MediaTime obj2) => obj1.TotalTicks <= obj2.TotalTicks;

    /// <summary>
    /// Compares two objects to determine whether the first specified object is not equal to the second specified object.
    /// Campara dos objetos para determinar si el primer objeto especificado no es igual al segundo objeto especificado.
    /// </summary>
    /// <param name="obj1">
    /// First specified object.
    /// Primer objeto especificado.
    /// </param>
    /// <param name="obj2">
    /// Second specified object.
    /// Segundo objeto especificado.
    /// </param>
    /// <returns>
    /// If the two objects are different, true; otherwise, false.
    /// Si los dos objetos son distintos true, en caso contrario false.
    /// </returns>
    public static bool operator !=(MediaTime obj1, MediaTime obj2) => !obj1.Equals(obj2);

    /// <summary>
    /// Subtracts two specified objects.
    /// Resta dos objetos especificados.
    /// </summary>
    /// <param name="obj1">
    /// First specified object.
    /// Primer objeto especificado.
    /// </param>
    /// <param name="obj2">
    /// Second specified object.
    /// Segundo objeto especificado.
    /// </param>
    /// <returns>
    /// Object resulting from the subtraction.
    /// Objeto resultante de la resta.
    /// </returns>
    public static MediaTime operator -(MediaTime obj1, MediaTime obj2)
    {
      MediaTime mt = new(obj1.TotalTicks, obj1.FractionsPerSeconds);
      mt.TotalTicks -= obj2.TotalTicks;
      return mt;
    }

    /// <summary>
    /// Adds two specified objects.
    /// Suma dos objetos especificados.
    /// </summary>
    /// <param name="obj1">
    /// First specified object.
    /// Primer objeto especificado.
    /// </param>
    /// <param name="obj2">
    /// Second specified object.
    /// Segundo objeto especificado.
    /// </param>
    /// <returns>
    /// Object resulting from the sum.
    /// Objeto resultante de la suma.
    /// </returns>
    public static MediaTime operator +(MediaTime obj1, MediaTime obj2)
    {
      MediaTime mt = new(obj1.TotalTicks, obj1.FractionsPerSeconds);
      mt.TotalTicks += obj2.TotalTicks;
      return mt;
    }

    /// <summary>
    /// Compares two objects to determine whether the first specified object is equal to the second specified object.
    /// Campara dos objetos para determinar si el primer objeto especificado es igual al segundo objeto especificado.
    /// </summary>
    /// <param name="obj1">
    /// First specified object.
    /// Primer objeto especificado.
    /// </param>
    /// <param name="obj2">
    /// Second specified object.
    /// Segundo objeto especificado.
    /// </param>
    /// <returns>
    /// If the two objects are equal, true; otherwise, false.
    /// Si los dos objetos son iguales true, en caso contrario false.
    /// </returns>
    public static bool operator ==(MediaTime obj1, MediaTime obj2) => obj1.Equals(obj2);

    /// <summary>
    /// Compares two objects to determine whether the first specified object is greater than the second specified object.
    /// Compara dos objetos para determinar si el primer objeto especificado es mayor que el segundo objeto especificado.
    /// </summary>
    /// <param name="obj1">
    /// First specified object.
    /// Primer objeto especificado.
    /// </param>
    /// <param name="obj2">
    /// Second specified object.
    /// Segundo objeto especificado.
    /// </param>
    /// <returns>
    /// If the first specified object is greater than the second specified object, true; otherwise, false.
    /// Si el primer objeto especificado es mayor que el segundo objeto especificado true, en caso contrario false.
    /// </returns>
    public static bool operator >(MediaTime obj1, MediaTime obj2) => obj1.TotalTicks > obj2.TotalTicks;

    /// <summary>
    /// Compares two objects to determine whether the first specified object is greater than or equal to the second specified object.
    /// Compara dos objetos para determinar si el primer objeto especificado es mayor o igual que el segundo objeto especificado.
    /// </summary>
    /// <param name="obj1">
    /// First specified object.
    /// Primer objeto especificado.
    /// </param>
    /// <param name="obj2">
    /// Second specified object.
    /// Segundo objeto especificado.
    /// </param>
    /// <returns>
    /// If the first specified object is greater than or equal to the second specified object, true; otherwise, false.
    /// Si el primer objeto especificado es mayor o igual que el segundo objeto especificado true, en caso contrario false.
    /// </returns>
    public static bool operator >=(MediaTime obj1, MediaTime obj2) => obj1.TotalTicks >= obj2.TotalTicks;
    #endregion

    #region Public methods
    /// <summary>
    /// Returns a <see cref="Espamatica.Core.MediaTime"/> representing a specified number of fractions with an approximate precision to the nearest millisecond.
    /// Devuelve un <see cref="Espamatica.Core.MediaTime"/> que representa un número de fracciones especificado con una precisión aproximada al milisegundo más cercano.
    /// </summary>
    /// <param name="fractions">
    /// Number of fractions with an accuracy of approximately the nearest millisecond.
    /// Número de fracciones con una precisión aproximada al milisegundo más cercano.
    /// </param>
    /// <param name="fractionsPerSecond">
    /// Fractions per second.
    /// Fracciones por segundo.
    /// </param>
    /// <param name="fractionRoundMode">
    /// Specifies the rounding mode for fractions.
    /// Indica el modo de redondeo de las fracciones.
    /// </param>
    /// <returns>
    /// <see cref="Espamatica.Core.MediaTime"/> representing the specified time.
    /// <see cref="Espamatica.Core.MediaTime"/> que representa el tiempo especificado.
    /// </returns>
    public static MediaTime FromFractions(long fractions, int fractionsPerSecond = DefaultFractionsPerSecond, FractionRoundMode fractionRoundMode = FractionRoundMode.Truncate) => new(fractions, fractionsPerSecond, fractionRoundMode);

    /// <summary>
    /// Converts the string representation of a time interval to its equivalent in <see cref="Espamatica.Core.MediaTime"/>.
    /// Convierte la representación de cadena de un intervalo de tiempo en su equivalente de <see cref="Espamatica.Core.MediaTime"/>. 
    /// </summary>
    /// <param name="s">
    /// String specifying the time interval to be converted. [hh:][mm:]ss[.fr].
    /// Cadena que especifica el intervalo de tiempo que se va a convertir. [hh:][mm:]ss[.fr].
    /// </param>
    /// <param name="fractionsPerSecond">
    /// Fractions per second.
    /// Fracciones por segundo.
    /// </param>
    /// <param name="fractionRoundMode">
    /// Indicates the rounding mode for fractions.
    /// Indica el modo de redondeo de las fracciones.
    /// </param>
    /// <returns>
    /// Object <see cref="Espamatica.Core.MediaTime"/> corresponding to s.
    /// Objeto <see cref="Espamatica.Core.MediaTime"/> que corresponde a s.
    /// </returns>
    public static MediaTime Parse(string s, int fractionsPerSecond, FractionRoundMode fractionRoundMode = FractionRoundMode.Truncate)
    {
      if (string.IsNullOrWhiteSpace(s))
        throw new ArgumentNullException(nameof(s));

      long hours = 0, minutes = 0, seconds, fractions = 0;
      string[] array = s.Split(['.']);

      if (array.Length > 2)
        throw new FormatException();
      else if (array.Length > 1)
      {
        fractions = long.Parse(array[1]);
        if (fractions > fractionsPerSecond)
          throw new OverflowException();
      }

      array = array[0].Split([':']);
      switch (array.Length)
      {
        case 1:
          seconds = Convert.ToInt64(array[0]) * Convert.ToInt64(fractionsPerSecond);
          break;
        case 2:
          minutes = Convert.ToInt64(array[0]) * 60L * Convert.ToInt64(fractionsPerSecond);
          seconds = Convert.ToInt64(array[1]) * Convert.ToInt64(fractionsPerSecond);
          break;
        case 3:
          hours = Convert.ToInt64(array[0]) * 3600L * Convert.ToInt64(fractionsPerSecond);
          minutes = Convert.ToInt64(array[1]) * 60L * Convert.ToInt64(fractionsPerSecond);
          seconds = Convert.ToInt64(array[2]) * Convert.ToInt64(fractionsPerSecond);
          break;
        default:
          throw new FormatException();
      }

      fractions += seconds + minutes + hours;

      return new MediaTime(fractions, fractionsPerSecond, fractionRoundMode);
    } // Parse

    /// <summary>
    /// Converts the string representation of a time interval to its equivalent in <see cref="Espamatica.Core.MediaTime"/>.
    /// Convierte la representación de cadena de un intervalo de tiempo en su equivalente de <see cref="Espamatica.Core.MediaTime"/>. 
    /// </summary>
    /// <param name="s">
    /// String specifying the time interval to be converted. [hh:][mm:]ss[.fr].
    /// Cadena que especifica el intervalo de tiempo que se va a convertir. [hh:][mm:]ss[.fr].
    /// </param>
    /// <param name="fractionsPerSecond">
    /// Fractions per second.
    /// Fracciones por segundo.
    /// </param>
    /// <param name="stringFormat">
    /// String format.
    /// Formato de la cadena.
    /// </param>
    /// <param name="fractionRoundMode">
    /// Indicates the rounding mode for fractions.
    /// Indica el modo de redondeo de las fracciones.
    /// </param>
    /// <param name="fractionSeparator">
    /// Character used for fraction separators.
    /// Carácter utilizado para el separador de fracciones.
    /// </param>
    /// <returns>
    /// Object <see cref="Espamatica.Core.MediaTime"/> corresponding to s.
    /// Objeto <see cref="Espamatica.Core.MediaTime"/> que corresponde a s.
    /// </returns>
    public static MediaTime Parse(string s, int fractionsPerSecond, MediaTimeStringFormat stringFormat, FractionRoundMode fractionRoundMode = FractionRoundMode.Truncate, char fractionSeparator = '.')
    {
      if (string.IsNullOrWhiteSpace(s))
        throw new ArgumentNullException(nameof(s));

      decimal hours = 0M, minutes = 0M, seconds = 0M, fractions = 0M;
      string decimalSeparator = CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator;
      string errorFormat = string.Empty;
      string[] array = s.Replace(fractionSeparator, ':').Split([':']);

      switch (stringFormat)
      {
        case MediaTimeStringFormat.HHmm:
          if (array.Length < 2 || array.Length > 4)
            errorFormat = $"{stringFormat}";
          else
          {
            hours = decimal.Parse(array[0]);
            minutes = decimal.Parse(array[1]);
          }
          break;
        case MediaTimeStringFormat.HHmmss:
          if (array.Length < 3 || array.Length > 4)
            errorFormat = $"{stringFormat}";
          else
          {
            hours = decimal.Parse(array[0]);
            minutes = decimal.Parse(array[1]);
            seconds = decimal.Parse(array[2]);
          }
          break;
        case MediaTimeStringFormat.HHmmssff:
          if (array.Length != 4)
            errorFormat = $"{stringFormat}";
          else
          {
            hours = decimal.Parse(array[0]);
            minutes = decimal.Parse(array[1]);
            seconds = decimal.Parse(array[2]);
            fractions = decimal.Parse(array[3]);
          }
          break;
        case MediaTimeStringFormat.HHmmssmmm:
          if (array.Length != 4)
            errorFormat = $"{stringFormat}";
          else
          {
            hours = decimal.Parse(array[0]);
            minutes = decimal.Parse(array[1]);
            seconds = decimal.Parse(string.Format("{0}{1}{2}", array[2], decimalSeparator, array[3]));
          }
          break;
        case MediaTimeStringFormat.mmss:
          if (array.Length < 2 || array.Length > 3)
            errorFormat = $"{stringFormat}";
          else
          {
            minutes = decimal.Parse(array[0]);
            seconds = decimal.Parse(array[1]);
          }
          break;
        case MediaTimeStringFormat.mmssff:
          if (array.Length != 3)
            errorFormat = $"{stringFormat}";
          else
          {
            minutes = decimal.Parse(array[0]);
            seconds = decimal.Parse(array[1]);
            fractions = decimal.Parse(array[2]);
          }
          break;
        case MediaTimeStringFormat.mmssmmm:
          if (array.Length != 3)
            errorFormat = $"{stringFormat}";
          else
          {
            minutes = decimal.Parse(array[0]);
            seconds = decimal.Parse(string.Format("{0}{1}{2}", array[1], decimalSeparator, array[2]));
          }
          break;
        case MediaTimeStringFormat.ssff:
          if (array.Length != 2)
            errorFormat = $"{stringFormat}";
          else
          {
            seconds = decimal.Parse(array[0]);
            fractions = decimal.Parse(array[1]);
          }
          break;
        case MediaTimeStringFormat.ssmmm:
          if (array.Length != 2)
            errorFormat = $"{stringFormat}";
          else
            seconds = decimal.Parse(string.Format("{0}{1}{2}", array[0], decimalSeparator, array[1]));
          break;
      }

      if (!string.IsNullOrWhiteSpace(errorFormat))
        throw new FormatException($"Invalid string format: {errorFormat}");

      seconds *= Convert.ToDecimal(fractionsPerSecond);
      minutes *= 60M * Convert.ToDecimal(fractionsPerSecond);
      hours *= 3600M * Convert.ToDecimal(fractionsPerSecond);
      fractions += seconds + minutes + hours;

      return FromFractions(Convert.ToInt64(fractions), fractionsPerSecond, fractionRoundMode);
    }

    /// <summary>
    /// Converts the string representation of a time interval to its equivalent in <see cref="Espamatica.Core.MediaTime"/> and returns a value indicating whether the conversion was successful.
    /// Convierte la representación de cadena de un intervalo de tiempo en su equivalente de <see cref="Espamatica.Core.MediaTime"/> y devuelve un valor que indica si la conversión se realizó correctamente.
    /// </summary>
    /// <param name="s">
    /// String specifying the time interval to be converted. [hh:][mm:]ss[.fr].
    /// Cadena que especifica el intervalo de tiempo que se va a convertir. [hh:][mm:]ss[.fr].
    /// </param>
    /// <param name="fractionsPerSecond">
    /// Fractions per second.
    /// Fracciones por segundo.
    /// </param>
    /// <param name="result">
    /// The result returned by this method contains an object representing the time interval specified by s or <see cref="System.TimeSpan.MinValue"/> if the conversion did not complete successfully. This parameter is passed uninitialized.
    /// El resultado que devuelve este método contiene un objeto que representa el intervalo de tiempo especificado por s o <see cref="System.TimeSpan.MinValue"/> si la conversión no finalizó correctamente. Este parámetro se pasa sin inicializar.
    /// </param>
    /// <param name="fractionRoundMode">
    /// Indicates the rounding mode for fractions.
    /// Indica el modo de redondeo de las fracciones.
    /// </param>
    /// <returns>
    /// It is true if s was converted correctly; otherwise, it is false.
    /// This operation returns false if the parameter s is Nothing or <see cref="String. Empty"/>, has an invalid format, represents a time span less than <see cref="System.TimeSpan.MinValue"/> or greater than <see cref="System.TimeSpan.MaxValue"/>,
    /// or has at least one of its day, hour, minute, second, or frame components outside the valid range.
    /// Es true si s se convirtió correctamente; de lo contrario, es false.
    /// Esta operación devuelve false si el parámetro s es Nothing o <see cref="String.Empty"/>, tiene un formato no válido, representa un intervalo de tiempo menor que <see cref="System.TimeSpan.MinValue"/> o mayor que <see cref="System.TimeSpan.MaxValue"/>, 
    /// o tiene al menos uno de sus componentes de días, horas, minutos, segundos o frames fuera del intervalo válido.
    /// </returns>
    public static bool TryParse(string s, int fractionsPerSecond, out MediaTime result, FractionRoundMode fractionRoundMode = FractionRoundMode.Truncate)
    {
      bool isOk = true;

      try
      {
        result = Parse(s, fractionsPerSecond, fractionRoundMode);
      }
      catch
      {
        result = new(0, fractionsPerSecond, fractionRoundMode);
        isOk = false;
      }

      return isOk;
    } // TryParse

    /// <summary>
    /// Converts the string representation of a time interval to its equivalent in <see cref="Espamatica.Core.MediaTime"/> and returns a value indicating whether the conversion was successful.
    /// Convierte la representación de cadena de un intervalo de tiempo en su equivalente de <see cref="Espamatica.Core.MediaTime"/> y devuelve un valor que indica si la conversión se realizó correctamente.
    /// </summary>
    /// <param name="s">
    /// String specifying the time interval to be converted. [hh:][mm:]ss[.fr].
    /// Cadena que especifica el intervalo de tiempo que se va a convertir. [hh:][mm:]ss[.fr].
    /// </param>
    /// <param name="fractionsPerSecond">
    /// Fractions per second.
    /// Fracciones por segundo.
    /// </param>
    /// <param name="result">
    /// The result returned by this method contains an object representing the time interval specified by s or <see cref="System.TimeSpan.MinValue"/> if the conversion did not complete successfully. This parameter is passed uninitialized.
    /// El resultado que devuelve este método contiene un objeto que representa el intervalo de tiempo especificado por s o <see cref="System.TimeSpan.MinValue"/> si la conversión no finalizó correctamente. Este parámetro se pasa sin inicializar.
    /// </param>
    /// <param name="stringFormat">
    /// Specifies the string format.
    /// Indica el formato de cadena.
    /// </param>
    /// <param name="fractionRoundMode">
    /// Indicates the rounding mode for fractions.
    /// Indica el modo de redondeo de las fracciones.
    /// </param>
    /// <param name="fractionSeparator">
    /// Specifies the character used as a fraction separator.
    /// Indica el carácter utilizado como separador de fracciones.
    /// </param>
    /// <returns>
    /// It is true if s was converted correctly; otherwise, it is false.
    /// This operation returns false if the parameter s is Nothing or <see cref="String. Empty"/>, has an invalid format, represents a time span less than <see cref="System.TimeSpan.MinValue"/> or greater than <see cref="System.TimeSpan.MaxValue"/>,
    /// or has at least one of its day, hour, minute, second, or frame components outside the valid range.
    /// Es true si s se convirtió correctamente; de lo contrario, es false.
    /// Esta operación devuelve false si el parámetro s es Nothing o <see cref="String.Empty"/>, tiene un formato no válido, representa un intervalo de tiempo menor que <see cref="System.TimeSpan.MinValue"/> o mayor que <see cref="System.TimeSpan.MaxValue"/>, 
    /// o tiene al menos uno de sus componentes de días, horas, minutos, segundos o frames fuera del intervalo válido.
    /// </returns>
    public static bool TryParse(string s, int fractionsPerSecond, out MediaTime result, MediaTimeStringFormat stringFormat, FractionRoundMode fractionRoundMode = FractionRoundMode.Truncate, char fractionSeparator = '.')
    {
      var isOk = true;

      try
      {
        result = Parse(s, fractionsPerSecond, stringFormat, fractionRoundMode, fractionSeparator);
      }
      catch
      {
        result = new(0, fractionsPerSecond, fractionRoundMode);
        isOk = false;
      }

      return isOk;
    }

    /// <summary>
    /// Adds the specified number of days to the current time.
    /// Añade el número de días especificado al tiempo actual.
    /// </summary>
    /// <param name="days">
    /// Specified number of days.
    /// Número de días especificado.
    /// </param>
    public void AddDays(int days) => TotalTicks += days * TicksDay;

    /// <summary>
    /// Adds the specified number of hours to the current time.
    /// Añade el número de horas especificado al tiempo actual.
    /// </summary>
    /// <param name="hours">
    /// Specified number of hours.
    /// Número de horas especificado.
    /// </param>
    public void AddHours(int hours) => TotalTicks += hours * TicksHour;

    /// <summary>
    /// Adds the specified number of fractions to the current time.
    /// Añade el número de fracciones especificado al tiempo actual.
    /// </summary>
    /// <param name="fractions">
    /// Specified number of fractions.
    /// Número de fracciones especificado.
    /// </param>
    public void AddFractions(int fractions) => TotalTicks += (long)(fractions * MillisecondsPerFraction * TicksMillisecond);

    /// <summary>
    /// Adds the specified number of milliseconds to the current time.
    /// Añade el número de milisegundos especificado al tiempo actual.
    /// </summary>
    /// <param name="milliseconds">
    /// Specified number of milliseconds.
    /// Número de milisegundos especificado.
    /// </param>
    public void AddMilliseconds(int milliseconds) => TotalTicks += milliseconds * TicksMillisecond;

    /// <summary>
    /// Adds the specified number of minutes to the current time.
    /// Añade el número de minutos especificado al tiempo actual.
    /// </summary>
    /// <param name="minutes">
    /// Specified number of minutes.
    /// Número de minutos especificado.
    /// </param>
    public void AddMinutes(int minutes) => TotalTicks += minutes * TicksMinute;

    /// <summary>
    /// Adds the specified number of seconds to the current time.
    /// Añade el número de segundos especificado al tiempo actual.
    /// </summary>
    /// <param name="seconds">
    /// Specified number of seconds.
    /// Número de segundos especificado.
    /// </param>
    public void AddSeconds(int seconds) => TotalTicks += seconds * TicksSecond;

    /// <summary>
    /// Adds the specified number of ticks to the current time.
    /// Añade el número de ticks especificado al tiempo actual.
    /// </summary>
    /// <param name="ticks">
    /// Specified number of ticks.
    /// Número de ticks especificado.
    /// </param>
    public void AddTicks(int ticks) => TotalTicks += ticks;

    /// <summary>
    /// Compares the specified object with the current instance to determine whether they are considered equal.
    /// Compara el objeto especificado con la instancia actual para determinar si se consideran iguales.
    /// </summary>
    /// <param name="obj">
    /// Specified object.
    /// Objeto especificado.
    /// </param>
    /// <returns>
    /// If the two objects are considered equal, true; otherwise, false.
    /// Si los dos objetos son considerados iguales true, en caso contrario false.
    /// </returns>
    public readonly override bool Equals(object? obj) => obj is MediaTime mediaTime && mediaTime.TotalTicks == TotalTicks;

    /// <summary>
    /// Gets the hash code for the current instance.
    /// Obtiene el código hash de la instancia actual.
    /// </summary>
    /// <returns>
    /// Hash code for the current instance.
    /// Código hash de la instancia actual.
    /// </returns>
    public readonly override int GetHashCode() => TotalTicks.GetHashCode();

    /// <summary>
    /// Gets the total number of days.
    /// Obtiene el número total de días.
    /// </summary>
    /// <returns>
    /// Total number of days.
    /// Número total de días.
    /// </returns>
    public readonly double GetTotalDays() => TotalTicks / (double)TicksDay;

    /// <summary>
    /// Gets the total number of fractions.
    /// Obtiene el número total de fracciones.
    /// </summary>
    /// <returns>
    /// Total number of fractions.
    /// Número total de fracciones.
    /// </returns>
    public readonly double GetTotalFractions() => TotalTicks / (double)TicksMillisecond / MillisecondsPerFraction;

    /// <summary>
    /// Gets the total number of hours.
    /// Obtiene el número total de horas.
    /// </summary>
    /// <returns>
    /// Total number of hours.
    /// Número total de horas.
    /// </returns>
    public readonly double GetTotalHours() => TotalTicks / (double)TicksHour;

    /// <summary>
    /// Gets the total number of milliseconds.
    /// Obtiene el número total de milisegundos.
    /// </summary>
    /// <returns>
    /// Total number of milliseconds.
    /// Número total de milisegundos.
    /// </returns>
    public readonly double GetTotalMilliseconds() => TotalTicks / (double)TicksMillisecond;

    /// <summary>
    /// Gets the total number of minutes.
    /// Obtiene el número total de minutos.
    /// </summary>
    /// <returns>
    /// Total number of minutes.
    /// Número total de minutos.
    /// </returns>
    public readonly double GetTotalMinutes() => TotalTicks / (double)TicksMinute;

    /// <summary>
    /// Gets the total number of seconds.
    /// Obtiene el número total de segundos.
    /// </summary>
    /// <returns>
    /// Total number of seconds.
    /// Número total de segundos.
    /// </returns>
    public readonly double GetTotalSeconds() => TotalTicks / (double)TicksSecond;

    /// <summary>
    /// Gets the current time string representation.
    /// Obtiene la representación en cadena del tiempo actual.
    /// </summary>
    /// <returns>
    /// Current time string representation.
    /// Representación en cadena del tiempo actual.
    /// </returns>
    public readonly override string ToString()
    {
      int fractionsRounded = RoundMode switch
      {
        FractionRoundMode.Real => this.fractions % 1 > 0.0 ? (int)Math.Truncate(fractions) + 1 : (int)fractions,
        FractionRoundMode.Round => (int)Math.Round(fractions, 0, MidpointRounding.AwayFromZero),
        FractionRoundMode.Truncate => (int)Math.Truncate(fractions),
        _ => (int)Math.Truncate(fractions),
      };

      return string.Format("{0:d2}:{1:d2}:{2:d2}:{3:d2}.{4}", [Days, Hours, Minutes, Seconds, $"{fractionsRounded}".PadLeft(DigitPerFraction, '0')]);
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Update the data relating to fractions per second.
    /// Actualiza los datos relativos a las fracciones por segundo.
    /// </summary>
    private void RefreshFractionPerSeconds()
    {
      if (FractionsPerSeconds > 0 && FractionsPerSeconds < (int)MillisecondsSecond)
      {
        MillisecondsPerFraction = Math.Round(1.0 / FractionsPerSeconds * MillisecondsSecond, 4);
        DigitPerFraction = $"{FractionsPerSeconds}".Length;

        if (Math.Pow(10.0, DigitPerFraction - 1.0) == FractionsPerSeconds)
          DigitPerFraction--;
      }
    }

    /// <summary>
    /// Update multimedia time values.
    /// Actualiza los valores del tiempo mutimedia.
    /// </summary>
    private void RefreshMediaTime()
    {
      long remainder = TotalTicks;

      days = (int)((remainder - (remainder % TicksDay)) / TicksDay);
      remainder -= days * TicksDay;

      hours = (int)((remainder - (remainder % TicksHour)) / TicksHour);
      remainder -= hours * TicksHour;

      minutes = (int)((remainder - (remainder % TicksMinute)) / TicksMinute);
      remainder -= minutes * TicksMinute;

      seconds = (int)((remainder - (remainder % TicksSecond)) / TicksSecond);
      remainder -= seconds * TicksSecond;

      fractions = ((double)remainder / TicksMillisecond) / MillisecondsPerFraction;
      fractions = double.IsNaN(fractions) ? 0.0 : fractions;

      milliseconds = (int)((remainder - (remainder % TicksMillisecond)) / TicksMillisecond);
      remainder -= milliseconds * TicksMillisecond;

      ticks = (int)remainder;
    }

    /// <summary>
    /// Update the time.
    /// Actualiza el tiempo.
    /// </summary>
    private void RefreshTime() => totalTicks = (Days * MediaTime.TicksDay) + (Hours * TicksHour) + (Minutes * TicksMinute) + (Seconds * TicksSecond) + (Milliseconds * TicksMillisecond) + Ticks;
    #endregion
  }
}
