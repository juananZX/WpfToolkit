namespace Espamatica.Core
{
  /// <summary>
  /// Estructura para manejo de tiempos multimedia.
  /// </summary>
  /// <see cref="https://espamatica.com/mediatime/"/>
  public struct MediaTime
  {
    #region Public consts
    /// <summary>
    /// Fracciones por defecto por segundo.
    /// </summary>
    public const int DefaultFractionsPerSecond = 1000;

    /// <summary>
    /// Número de ticks por milisegundo.
    /// </summary>
    public const long TicksMillisecond = 10000;

    /// <summary>
    /// Número de milisegundos por segundo.
    /// </summary>
    public const long MillisecondsSecond = 1000;

    /// <summary>
    /// Número de segundos por minuto.
    /// </summary>
    public const long SecondsMinute = 60;

    /// <summary>
    /// Número de minutos por hora.
    /// </summary>
    public const long MinutesHour = 60;

    /// <summary>
    /// Número de horas por día.
    /// </summary>
    public const long HoursDay = 24;

    /// <summary>
    /// Número de ticks por segundo.
    /// </summary>
    public const long TicksSecond = TicksMillisecond * MillisecondsSecond;

    /// <summary>
    /// Número de ticks por minuto.
    /// </summary>
    public const long TicksMinute = TicksSecond * SecondsMinute;

    /// <summary>
    /// Número de ticks por hora.
    /// </summary>
    public const long TicksHour = TicksMinute * MinutesHour;

    /// <summary>
    /// Número de ticks por día.
    /// </summary>
    public const long TicksDay = TicksHour * HoursDay;
    #endregion

    #region Private fields
    /// <summary>
    /// Campo de control para la propiedad Days.
    /// </summary>
    private int days;

    /// <summary>
    /// Campo de control para la propiedad DigitPerFraction.
    /// </summary>
    private int digitPerFraction;

    /// <summary>
    /// Campo de control para la propiedad Fractions.
    /// </summary>
    private double fractions;

    /// <summary>
    /// Campo de control para la propiedad FractionsPerSecond.
    /// </summary>
    private int fractionsPerSecond;

    /// <summary>
    /// Campo de control para la propiedad Hours.
    /// </summary>
    private int hours;

    /// <summary>
    /// Campo de control para la propiedad Milliseconds.
    /// </summary>
    private int milliseconds;

    /// <summary>
    /// Campo de control para la propiedad MillisecondsPerFraction.
    /// </summary>
    private double millisecondsPerFraction;

    /// <summary>
    /// Campo de control para la propiedad Minutes.
    /// </summary>
    private int minutes;

    /// <summary>
    /// Campo de control para la propiedad RoundMode.
    /// </summary>
    private FractionRoundMode roundMode;

    /// <summary>
    /// Campo de control para la propiedad Seconds.
    /// </summary>
    private int seconds;

    /// <summary>
    /// Campo de control para la propiedad Ticks.
    /// </summary>
    private int ticks;

    /// <summary>
    /// Campo de control para la propiedad Ticks.
    /// </summary>
    private long totalTicks;
    #endregion

    #region Constructors
    /// <summary>
    /// Inicializa una nueva instancia de la estructura MediaTime.
    /// </summary>
    /// <param name="time">Tiempo en ticks.</param>
    /// <param name="fractionsPerSecond">Fracciones por segundo (entre 1 y 1000).</param>
    public MediaTime(long time, int fractionsPerSecond)
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
      roundMode = FractionRoundMode.Truncate;
      seconds = 0;
      ticks = 0;
      totalTicks = time;

      RefreshFractionPerSeconds();
      RefreshMediaTime();
    }
    #endregion

    #region Public properties
    /// <summary>
    /// Obtiene o establece los días.
    /// </summary>
    public int Days
    {
      get => days;
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
    /// Obtiene el número de dígitos usados para representar las fracciones.
    /// </summary>
    public int DigitPerFraction
    {
      get => digitPerFraction;
      private set => digitPerFraction = value;
    }

    /// <summary>
    /// Obtiene o establece las fracciones.
    /// </summary>
    public double Fractions
    {
      get => fractions;
      set
      {
        if (value != fractions)
          Milliseconds = (int)Math.Truncate(value * MillisecondsPerFraction);
      }
    }

    /// <summary>
    /// Obtiene o establece el número de fracciones por segundo.
    /// </summary>
    public int FractionsPerSeconds
    {
      get => fractionsPerSecond;
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
    /// Obtiene o establece las horas.
    /// </summary>
    public int Hours
    {
      get => hours;
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
    /// Obtiene o establece los milisegundos.
    /// </summary>
    public int Milliseconds
    {
      get => milliseconds;
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
    /// Obtiene el número de milisegundos por fracción.
    /// </summary>
    public double MillisecondsPerFraction
    {
      get => millisecondsPerFraction;
      private set => millisecondsPerFraction = value;
    }

    /// <summary>
    /// Obtiene o establece los minutos.
    /// </summary>
    public int Minutes
    {
      get => minutes;
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
    /// Obtiene o establece el modo en el que se redondean las fracciones a la hora de representarlas en texto.
    /// </summary>
    public FractionRoundMode RoundMode
    {
      get => roundMode;
      set => roundMode = value;
    }

    /// <summary>
    /// Obtiene o establece los segundos.
    /// </summary>
    public int Seconds
    {
      get => seconds;
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
    /// Obtiene o establece los ticks.
    /// </summary>
    public int Ticks
    {
      get => ticks;
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
    /// Obtiene o establece el tiempo en ticks.
    /// </summary>
    public long TotalTicks
    {
      get => totalTicks;
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
    /// Compara dos objetos para determinar si el primer objeto especificado es menor que el segundo objeto especificado.
    /// </summary>
    /// <param name="obj1">Primer objeto especificado.</param>
    /// <param name="obj2">Segundo objeto especificado.</param>
    /// <returns>Si el primer objeto especificado es menos que el segundo objeto especificado true, en caso contrario false.</returns>
    public static bool operator <(MediaTime obj1, MediaTime obj2) => obj1.TotalTicks < obj2.TotalTicks;

    /// <summary>
    /// Compara dos objetos para determinar si el primer objeto especificado es menor o igual que el segundo objeto especificado.
    /// </summary>
    /// <param name="obj1">Primer objeto especificado.</param>
    /// <param name="obj2">Segundo objeto especificado.</param>
    /// <returns>Si el primer objeto especificado es menor o igual que el segundo objeto especificado true, en caso contrario false.</returns>
    public static bool operator <=(MediaTime obj1, MediaTime obj2) => obj1.TotalTicks <= obj2.TotalTicks;

    /// <summary>
    /// Campara dos objetos para determinar si el primer objeto especificado no es igual al segundo objeto especificado.
    /// </summary>
    /// <param name="obj1">Primer objeto especificado.</param>
    /// <param name="obj2">Segundo objeto especificado.</param>
    /// <returns>Si los dos objetos son distintos true, en caso contrario false.</returns>
    public static bool operator !=(MediaTime obj1, MediaTime obj2) => !obj1.Equals(obj2);

    /// <summary>
    /// Resta dos objetos especificados.
    /// </summary>
    /// <param name="obj1">Primero objeto especificado.</param>
    /// <param name="obj2">Segundo objeto especificado.</param>
    /// <returns>Objeto resultante de la resta.</returns>
    public static MediaTime operator -(MediaTime obj1, MediaTime obj2)
    {
      MediaTime mt = new(obj1.TotalTicks, obj1.FractionsPerSeconds);
      mt.TotalTicks -= obj2.TotalTicks;
      return mt;
    }

    /// <summary>
    /// Suma dos objetos especificados.
    /// </summary>
    /// <param name="obj1">Primero objeto especificado.</param>
    /// <param name="obj2">Segundo objeto especificado.</param>
    /// <returns>Objeto resultante de la suma.</returns>
    public static MediaTime operator +(MediaTime obj1, MediaTime obj2)
    {
      MediaTime mt = new(obj1.TotalTicks, obj1.FractionsPerSeconds);
      mt.TotalTicks += obj2.TotalTicks;
      return mt;
    }

    /// <summary>
    /// Campara dos objetos para determinar si el primer objeto especificado es igual al segundo objeto especificado.
    /// </summary>
    /// <param name="obj1">Primer objeto especificado.</param>
    /// <param name="obj2">Segundo objeto especificado.</param>
    /// <returns>Si los dos objetos son iguales true, en caso contrario false.</returns>
    public static bool operator ==(MediaTime obj1, MediaTime obj2) => obj1.Equals(obj2);

    /// <summary>
    /// Compara dos objetos para determinar si el primer objeto especificado es mayor que el segundo objeto especificado.
    /// </summary>
    /// <param name="obj1">Primer objeto especificado.</param>
    /// <param name="obj2">Segundo objeto especificado.</param>
    /// <returns>Si el primer objeto especificado es mayor que el segundo objeto especificado true, en caso contrario false.</returns>
    public static bool operator >(MediaTime obj1, MediaTime obj2) => obj1.TotalTicks > obj2.TotalTicks;

    /// <summary>
    /// Compara dos objetos para determinar si el primer objeto especificado es mayor o igual que el segundo objeto especificado.
    /// </summary>
    /// <param name="obj1">Primer objeto especificado.</param>
    /// <param name="obj2">Segundo objeto especificado.</param>
    /// <returns>Si el primer objeto especificado es mayor o igual que el segundo objeto especificado true, en caso contrario false.</returns>
    public static bool operator >=(MediaTime obj1, MediaTime obj2) => obj1.TotalTicks >= obj2.TotalTicks;
    #endregion

    #region Public methods
    /// <summary>
    /// Añade el número de días especificado al tiempo actual.
    /// </summary>
    /// <param name="days">Número de días especificado.</param>
    public void AddDays(int days) => TotalTicks += days * TicksDay;

    /// <summary>
    /// Añade el número de horas especificado al tiempo actual.
    /// </summary>
    /// <param name="hours">Número de horas especificado.</param>
    public void AddHours(int hours) => TotalTicks += hours * TicksHour;

    /// <summary>
    /// Añade el número de fracciones especificado al tiempo actual.
    /// </summary>
    /// <param name="fractions">Número de fracciones especificado.</param>
    public void AddFractions(int fractions) => TotalTicks += (long)(fractions * MillisecondsPerFraction * TicksMillisecond);

    /// <summary>
    /// Añade el número de milisegundos especificado al tiempo actual.
    /// </summary>
    /// <param name="milliseconds">Número de milisegundos especificado.</param>
    public void AddMilliseconds(int milliseconds) => TotalTicks += milliseconds * TicksMillisecond;

    /// <summary>
    /// Añade el número de minutos especificado al tiempo actual.
    /// </summary>
    /// <param name="minutes">Número de minutos especificado.</param>
    public void AddMinutes(int minutes) => TotalTicks += minutes * TicksMinute;

    /// <summary>
    /// Añade el número de segundos especificado al tiempo actual.
    /// </summary>
    /// <param name="seconds">Número de segundos especificado.</param>
    public void AddSeconds(int seconds) => TotalTicks += seconds * TicksSecond;

    /// <summary>
    /// Añade el número de ticks especificado al tiempo actual.
    /// </summary>
    /// <param name="ticks">Número de ticks especificado.</param>
    public void AddTicks(int ticks) => TotalTicks += ticks;

    /// <summary>
    /// Compara el objeto especificado con la instacia actual para determinar si se consideran iguales.
    /// </summary>
    /// <param name="obj">Objeto especificado.</param>
    /// <returns>Si los dos objetos son considerados iguales true, en caso contrario false.</returns>
    public override bool Equals(object? obj) => obj is MediaTime mediaTime && mediaTime.TotalTicks == TotalTicks;

    /// <summary>
    /// Obtiene el código hash de la instancia actual.
    /// </summary>
    /// <returns>Código hash de la instancia actual.</returns>
    public override int GetHashCode() => TotalTicks.GetHashCode();

    /// <summary>
    /// Obtiene el número total de días.
    /// </summary>
    /// <returns>Número total de días.</returns>
    public double GetTotalDays() => TotalTicks / (double)TicksDay;

    /// <summary>
    /// Obtiene el número total de fracciones.
    /// </summary>
    /// <returns>Número total de fracciones.</returns>
    public double GetTotalFractions() => TotalTicks / (double)TicksMillisecond / MillisecondsPerFraction;

    /// <summary>
    /// Obtiene el número total de horas.
    /// </summary>
    /// <returns>Número total de horas.</returns>
    public double GetTotalHours() => TotalTicks / (double)TicksHour;

    /// <summary>
    /// Obtiene el número total de milisegundos.
    /// </summary>
    /// <returns>Número total de milisegundos.</returns>
    public double GetTotalMilliseconds() => TotalTicks / (double)TicksMillisecond;

    /// <summary>
    /// Obtiene el número total de minutos.
    /// </summary>
    /// <returns>Número total de minutos.</returns>
    public double GetTotalMinutes() => TotalTicks / (double)TicksMinute;

    /// <summary>
    /// Obtiene el número total de segundos.
    /// </summary>
    /// <returns>Número total de segundos.</returns>
    public double GetTotalSeconds() => TotalTicks / (double)TicksSecond;

    /// <summary>
    /// Obtiene la representación en cadena del tiempo actual.
    /// </summary>
    /// <returns>Representación en cadena del tiempo actual.</returns>
    public override string ToString()
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

      milliseconds = (int)((remainder - (remainder % TicksMillisecond)) / TicksMillisecond);
      remainder -= milliseconds * TicksMillisecond;

      ticks = (int)remainder;
    }

    /// <summary>
    /// Actualiza el tiempo.
    /// </summary>
    private void RefreshTime() => totalTicks = (Days * MediaTime.TicksDay) + (Hours * TicksHour) + (Minutes * TicksMinute) + (Seconds * TicksSecond) + (Milliseconds * TicksMillisecond) + Ticks;
    #endregion
  }
}
