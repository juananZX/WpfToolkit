using Espamatica.Core;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Espamatica.WpfToolkit
{
  /// <summary>
  /// Multimedia times text box.
  /// Caja de texto de tiempos multimedia.
  /// </summary>
  public partial class MediaTimeTextBox : UserControl
  {
    public MediaTimeTextBox()
    {
      InitializeComponent();
    }

    #region Routed events
    public static readonly RoutedEvent TimeChangedEvent = EventManager.RegisterRoutedEvent(nameof(TimeChanged), RoutingStrategy.Direct, typeof(MediaTimeTextBoxTimeEventHandler), typeof(MediaTimeTextBox));
    #endregion

    #region Dependency properties
    public static readonly DependencyProperty AutoValidateTimeProperty = DependencyProperty.Register(nameof(AutoValidateTime), typeof(bool), typeof(MediaTimeTextBox), new PropertyMetadata(true));

    public static readonly DependencyProperty FillFormatProperty = DependencyProperty.Register(nameof(FillFormat), typeof(bool), typeof(MediaTimeTextBox), new PropertyMetadata(false, OnFillFormatChanged));

    public static readonly DependencyProperty FractionsPerSecondProperty = DependencyProperty.Register(nameof(FractionsPerSecond), typeof(int), typeof(MediaTimeTextBox), new PropertyMetadata(MediaTime.DefaultFractionsPerSecond, OnFractionsPerSecondChanged));

    public static readonly DependencyProperty FractionRoundModeProperty = DependencyProperty.Register(nameof(FractionRoundMode), typeof(FractionRoundMode), typeof(MediaTimeTextBox), new PropertyMetadata(FractionRoundMode.Truncate, OnFractionRoundModeChanged));

    public static readonly DependencyProperty FractionSeparatorProperty = DependencyProperty.Register(nameof(FractionSeparator), typeof(char), typeof(MediaTimeTextBox), new PropertyMetadata('.', OnFractionSeparatorChanged));

    public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(MediaTimeTextBox), new PropertyMetadata(false));

    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(MediaTimeTextBox), new PropertyMetadata(string.Empty, OnTextChanged));

    public static readonly DependencyProperty TimeFormatProperty = DependencyProperty.Register(nameof(TimeFormat), typeof(MediaTimeStringFormat), typeof(MediaTimeTextBox), new PropertyMetadata(MediaTimeStringFormat.HHmmssff, OnTimeFormatChanged));

    public static readonly DependencyProperty TimeProperty = DependencyProperty.Register(nameof(Time), typeof(long), typeof(MediaTimeTextBox), new PropertyMetadata(0L, OnTimeChanged));
    #endregion

    #region Private fields
    /// <summary>
    /// Field to control time internally.
    /// Campo para controlar el tiempo internamente.
    /// </summary>
    private long internalTime = 0L;

    /// <summary>
    /// Field to control whether the visual time is refreshed.
    /// Campo para controlar si se refresca el tiempo visual.
    /// </summary>
    private bool refreshVisualTime = true;

    /// <summary>
    /// Field to check whether a time change is taking place.
    /// Campo para controlar si se está llevando a cabo un cambio de tiempo.
    /// </summary>
    private bool changingTime = false;
    #endregion

    #region Public delegates
    /// <summary>
    /// Delegate for the MediaTimeTextBoxTimeEvent event.
    /// Delegado para el evento MediaTimeTextBoxTimeEvent.
    /// </summary>
    /// <param name="sender">
    /// Object that fires the event.
    /// Objeto que desencadena el evento.
    /// </param>
    /// <param name="e">
    /// Event details.
    /// Datos del evento.
    /// </param>
    public delegate void MediaTimeTextBoxTimeEventHandler(object sender, MediaTimeTextBoxTimeEventArgs e);
    #endregion

    #region Public events
    /// <summary>
    /// Event that notifies a change in time.
    /// Evento que notifica un cambio en el tiempo.
    /// </summary>
    public event MediaTimeTextBoxTimeEventHandler TimeChanged
    {
      add => AddHandler(TimeChangedEvent, value);
      remove => RemoveHandler(MediaTimeTextBox.TimeChangedEvent, value);
    }
    #endregion

    #region Public properties
    /// <summary>
    /// Gets or sets whether the time is self-validated.
    /// Obtiene o establece si se autovalida el tiempo.
    /// </summary>
    public bool AutoValidateTime
    {
      get => (bool)GetValue(AutoValidateTimeProperty);
      set => SetValue(MediaTimeTextBox.AutoValidateTimeProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the format is filled in the visual representation of the values.
    /// Obtiene o establece si se rellena el formato en la representación visual de los valores.
    /// </summary>
    public bool FillFormat
    {
      get => (bool)GetValue(FillFormatProperty);    
      set => SetValue(FillFormatProperty, value);
    }

    /// <summary>
    /// Gets or sets the number of fractions per second.
    /// Obtiene o establece el número de fracciones por segundo.
    /// </summary>
    public int FractionsPerSecond
    {
      get => (int)GetValue(FractionsPerSecondProperty);
      set => SetValue(FractionsPerSecondProperty, value);
    }

    /// <summary>
    /// Gets or sets the fraction adjustment mode.
    /// Obtiene o establece el modo de ajuste de las fracciones.
    /// </summary>
    public FractionRoundMode FractionRoundMode
    {
      get => (FractionRoundMode)GetValue(FractionRoundModeProperty);    
      set => SetValue(FractionRoundModeProperty, value);
    }

    /// <summary>
    /// Gets or sets the character that represents the fraction separator.
    /// Obtiene o establece el carácter que representa el separador de las fracciones.
    /// </summary>
    public char FractionSeparator
    {
      get => (char)GetValue(FractionSeparatorProperty);
      set => SetValue(FractionSeparatorProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the control is in read-only mode.
    /// Obtiene o establece si el control está en modo solo lectura.
    /// </summary>
    public bool IsReadOnly
    {
      get => (bool)GetValue(IsReadOnlyProperty);
      set => SetValue(IsReadOnlyProperty, value);
    }

    /// <summary>
    /// Gets or sets the text.
    /// Obtiene o establece el texto.
    /// </summary>
    public string Text
    {
      get => $"{GetValue(TextProperty)}";
      set => SetValue(TextProperty, value);
    }

    /// <summary>
    /// Gets or sets the time format.
    /// Obtiene o establece el formato del tiempo.
    /// </summary>
    public MediaTimeStringFormat TimeFormat
    {
      get => (MediaTimeStringFormat)GetValue(TimeFormatProperty);
      set => SetValue(TimeFormatProperty, value);
    }

    /// <summary>
    /// Gets or sets the ticks.
    /// Obtiene o establece los ticks.
    /// </summary>
    public long Time
    {
      get => (long)GetValue(TimeProperty);
      set  => SetValue(TimeProperty, value);
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Callback for the FillFormat property.
    /// Callback de la propiedad FillFormat.
    /// </summary>
    /// <param name="sender">
    /// Object that fires the event.
    /// Objeto que desencadena el evento.
    /// </param>
    /// <param name="e">
    /// Event details.
    /// Datos del evento.
    /// </param>
    private static void OnFillFormatChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => (sender as MediaTimeTextBox)?.OnFillFormatChanged(e);

    /// <summary>
    /// Callback for the FractionsPerSecond property.
    /// Callback de la propiedad FractionsPerSecond.
    /// </summary>
    /// <param name="sender">
    /// Object that fires the event.
    /// Objeto que desencadena el evento.
    /// </param>
    /// <param name="e">
    /// Event details.
    /// Datos del evento.
    /// </param>
    private static void OnFractionsPerSecondChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => (sender as MediaTimeTextBox)?.OnFractionsPerSecondChanged(e);

    /// <summary>
    /// Callback for the FractionRoundMode property.
    /// Callback de la propiedad FractionRoundMode.
    /// </summary>
    /// <param name="sender">
    /// Object that fires the event.
    /// Objeto que desencadena el evento.
    /// </param>
    /// <param name="e">
    /// Event details.
    /// Datos del evento.
    /// </param>
    private static void OnFractionRoundModeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => (sender as MediaTimeTextBox)?.OnFractionRoundModeChanged(e);

    /// <summary>
    /// Callback for the FractionSeparator property.
    /// Callback de la propiedad FractionSeparator.
    /// </summary>
    /// <param name="sender">
    /// Object that fires the event.
    /// Objeto que desencadena el evento.
    /// </param>
    /// <param name="e">
    /// Event details.
    /// Datos del evento.
    /// </param>
    private static void OnFractionSeparatorChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => (sender as MediaTimeTextBox)?.OnFractionSeparatorChanged(e);

    /// <summary>
    /// Callback for the Text property.
    /// Callback de la propiedad Text.
    /// </summary>
    /// <param name="sender">
    /// Object that fires the event.
    /// Objeto que desencadena el evento.
    /// </param>
    /// <param name="e">
    /// Event details.
    /// Datos del evento.
    /// </param>
    private static void OnTextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => (sender as MediaTimeTextBox)?.OnTextChanged(e);

    /// <summary>
    /// Callback for the Time property.
    /// Callback de la propiedad Time.
    /// </summary>
    /// <param name="sender">
    /// Object that fires the event.
    /// Objeto que desencadena el evento.
    /// </param>
    /// <param name="e">
    /// Event details.
    /// Datos del evento.
    /// </param>
    private static void OnTimeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => (sender as MediaTimeTextBox)?.OnTimeChanged(e);

    /// <summary>
    /// Callback for the TimeFormat property.
    /// Callback de la propiedad TimeFormat.
    /// </summary>
    /// <param name="sender">
    /// Object that fires the event.
    /// Objeto que desencadena el evento.
    /// </param>
    /// <param name="e">
    /// Event details.
    /// Datos del evento.
    /// </param>
    private static void OnTimeFormatChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => (sender as MediaTimeTextBox)?.OnTimeFormatChanged(e);

    /// <summary>
    /// Controls the action to be performed when a cursor key is pressed on the text boxes.
    /// Controla la acción a realizar cuando se pulsa alguna tecla del cursor sobre las cajas de texto.
    /// </summary>
    /// <param name="e">
    /// Event details.
    /// Datos del evento.
    /// </param>
    /// <param name="nextBox">
    /// Box that can bring the focus forward.
    /// Caja que puede tomar el foco hacia adelante.
    /// </param>
    /// <param name="previousBox">
    /// Box that can bring the focus backwards.
    /// Caja que puede tomar el foco hacia atrás.
    /// </param>
    private void ActionArrow(KeyEventArgs e, TextBox? nextBox, TextBox? previousBox)
    {
      long newTime, oldTime;

      if (e.OriginalSource is TextBox box && !IsReadOnly)
      {
        long factor = 0L, value = 1L;

        if (box == HoursBox)
          factor = TimeSpan.FromHours(value).Ticks;
        else if (box == this.MinutesBox)
          factor = TimeSpan.FromMinutes(value).Ticks;
        else if (box == this.SecondsBox)
          factor = TimeSpan.FromSeconds(value).Ticks;
        else if (box == this.FractionsBox)
          factor = MediaTime.FromFractions(value, this.FractionsPerSecond, this.FractionRoundMode).Ticks;

        switch (e.Key)
        {
          case Key.Down:
            oldTime = internalTime;
            internalTime = internalTime - factor < 0L ? 0L : internalTime - factor;
            newTime = internalTime;
            if (AutoValidateTime) NotifyTimeChanged(newTime, oldTime);
            SetVisualTime();
            break;
          case Key.Left:
            if (box.SelectionStart < 1 && box != HoursBox && previousBox != null)
            { 
              previousBox.Focus();
              previousBox.SelectionStart = previousBox.Text.Length;
              e.Handled = true;
            }
            break;
          case Key.Right:
            if (box.SelectionStart >= box.Text.Length && box != FractionsBox && nextBox != null)
            {
              nextBox.Focus();
              nextBox.SelectionStart = 0;
              e.Handled = true;
            }
            break;
          case Key.Up:
            oldTime = internalTime;
            internalTime += factor;
            newTime = internalTime;
            if (AutoValidateTime) NotifyTimeChanged(newTime, oldTime);
            SetVisualTime();
            break;
        }
      }
    }

    /// <summary>
    /// Controls the action to be performed when a key is pressed on the text boxes.
    /// Controla la acción a realizar cuando se pulsa alguna tecla sobre las cajas de texto.
    /// </summary>
    /// <param name="e">
    /// Event details.
    /// Datos del evento.
    /// </param>
    private void ActionKey(KeyEventArgs e)
    {
      TextBox? nextBox = null;
      TextBox? previousBox = null;
      int selectionStart = 0;

      if (e.OriginalSource is TextBox box)
      {
        // Assigns which box can take focus
        // Asigna que caja puede tomar el foco
        if (box == HoursBox)
        {
          if (MinutesBox.Visibility == Visibility.Visible)
            nextBox = MinutesBox;
        }
        else if (box == MinutesBox)
        {
          if (SecondsBox.Visibility == Visibility.Visible)
            nextBox = SecondsBox;
          if (HoursBox.Visibility == Visibility.Visible)
            previousBox = HoursBox;
        }
        else if (box == SecondsBox)
        {
          if (FractionsBox.Visibility == Visibility.Visible)
            nextBox = FractionsBox;
          if (MinutesBox.Visibility == Visibility.Visible)
            previousBox = MinutesBox;
        }
        else if (box == FractionsBox)
        {
          if (SecondsBox.Visibility == Visibility.Visible)
            previousBox = SecondsBox;
        }

        // Process the pressed key
        // Procesa la tecla pulsada
        switch (e.Key)
        {
          case Key.Back:
            if (box.SelectionStart != 0)
            {
              selectionStart = box.SelectionStart - 1;
              box.Text = box.Text.Remove(selectionStart, 1);
              box.Text = box.Text.Insert(selectionStart, "0");
              box.SelectionStart = selectionStart;
              e.Handled = true;
            }
            else if (box.SelectionStart == 0 && box != HoursBox && previousBox != null)
            {
              previousBox.Focus();
              previousBox.SelectionStart = previousBox.Text.Length;
              e.Handled = true;
            }
            break;
          case Key.Delete:
            if (box.SelectionStart < box.Text.Length)
            {
              selectionStart = box.SelectionStart;
              box.Text = box.Text.Remove(selectionStart, 1);
              box.Text = box.Text.Insert(selectionStart, "0");
              box.SelectionStart = selectionStart + 1;
              e.Handled = true;
            }
            else if (box.SelectionStart >= box.Text.Length && box != FractionsBox && nextBox != null)
            {
              nextBox.Focus();
              nextBox.SelectionStart = 0;
              e.Handled = true;
            }
            break;
          case Key.D0:
          case Key.D1:
          case Key.D2:
          case Key.D3:
          case Key.D4:
          case Key.D5:
          case Key.D6:
          case Key.D7:
          case Key.D8:
          case Key.D9:
          case Key.NumPad0:
          case Key.NumPad1:
          case Key.NumPad2:
          case Key.NumPad3:
          case Key.NumPad4:
          case Key.NumPad5:
          case Key.NumPad6:
          case Key.NumPad7:
          case Key.NumPad8:
          case Key.NumPad9:
            if (box.SelectionStart >= box.MaxLength && box != FractionsBox && nextBox != null)
            {
              nextBox.Focus();

              if (nextBox.Text.Length > 0)
              {
                nextBox.SelectionStart = 0;
                nextBox.SelectionLength = 1;
              }
            }
            else if (!IsReadOnly)
            {
              selectionStart = box.SelectionStart;

              if (box.Text.Length > selectionStart)
              {
                box.Text = box.Text.Remove(selectionStart, 1);
                box.SelectionStart = selectionStart;
              }
            }
            break;
          case Key.Down:
          case Key.Left:
          case Key.Right:
          case Key.Up:
            internalTime = GetVisualTime();
            ActionArrow(e, nextBox, previousBox);
            break;
          case Key.Enter:
            long oldTime = Time;
            internalTime = GetVisualTime();
            long newTime = internalTime;
            refreshVisualTime = false;
            SetTime(internalTime);
            SetText();
            refreshVisualTime = true;
            SetVisualTime();
            NotifyTimeChanged(newTime, oldTime);
            break;
        }
      }
    }

    /// <summary>
    /// Gets the string representation of the time.
    /// Obtiene la representación en cadena del tiempo.
    /// </summary>
    /// <returns>
    /// The string representation of the time.
    /// Representación en cadena del tiempo.
    /// </returns>
    private string GetTimeString()
    {
      MediaTime mt = MediaTime.FromFractions(internalTime, FractionsPerSecond, FractionRoundMode);

      string value = TimeFormat switch
      {
        MediaTimeStringFormat.HHmm => string.Format("{0:D2}:{1:D2}", mt.TotalHours, mt.Minutes),
        MediaTimeStringFormat.HHmmss => string.Format("{0:D2}:{1:D2}:{2:D2}", mt.TotalHours, mt.Minutes, mt.Seconds),
        MediaTimeStringFormat.HHmmssmmm => string.Format("{0:D2}:{1:D2}:{2:D2}" + $"{FractionSeparator}" + "{3:D3}", mt.TotalHours, mt.Minutes, mt.Seconds, mt.Milliseconds),
        MediaTimeStringFormat.mmss => string.Format("{0:D2}:{1:D2}", mt.TotalMinutes, mt.Seconds),
        MediaTimeStringFormat.mmssff => string.Format("{0:D2}:{1:D2}{2}", mt.TotalMinutes, mt.Seconds, mt.DigitPerFraction > 0 ? $"{FractionSeparator}" + mt.Fractions.ToString(CultureInfo.CurrentCulture).PadLeft(mt.DigitPerFraction, '0').PadRight(mt.DigitPerFraction, '0') : string.Empty),
        MediaTimeStringFormat.mmssmmm => string.Format("{0:D2}:{1:D2}" + $"{FractionSeparator}" + "{2:D3}", mt.TotalMinutes, mt.Seconds, mt.Milliseconds),
        MediaTimeStringFormat.ssff => string.Format("{0:D2}{1}", mt.TotalSeconds, mt.DigitPerFraction > 0 ? $"{FractionSeparator}" + mt.Fractions.ToString(CultureInfo.CurrentCulture).PadLeft(mt.DigitPerFraction, '0').PadRight(mt.DigitPerFraction, '0') : string.Empty),
        MediaTimeStringFormat.ssmmm => string.Format("{0:D2}" + $"{FractionSeparator}" + "{1:D3}", mt.TotalSeconds, mt.Milliseconds),
        _ => string.Format("{0:D2}:{1:D2}:{2:D2}{3}", mt.TotalHours, mt.Minutes, mt.Seconds, mt.DigitPerFraction > 0 ? $"{FractionSeparator}" + mt.Fractions.ToString(CultureInfo.CurrentCulture).PadLeft(mt.DigitPerFraction, '0').PadRight(mt.DigitPerFraction, '0') : string.Empty),
      };

      return value;
    }

    /// <summary>
    /// Gets the time from visual values.
    /// Obtiene el tiempo desde los valores visuales.
    /// </summary>
    /// <returns>
    /// Time represented by visual values.
    /// Tiempo representado por los valores visuales.
    /// </returns>
    private long GetVisualTime() => this.GetVisualTime(FractionsPerSecond, FractionRoundMode);

    /// <summary>
    /// Gets the time from visual values.
    /// Obtiene el tiempo desde los valores visuales.
    /// </summary>
    /// <param name="fractionsPerSecond">
    /// Number of fractions per second.
    /// Número de fracciones por segundo.
    /// </param>
    /// <param name="fractionRoundMode">
    /// Fraction rounding mode.
    /// Modo de redondeo de las fracciones.
    /// </param>
    /// <returns>
    /// Time represented by visual values.
    /// Tiempo representado por los valores visuales.
    /// </returns>
    private long GetVisualTime(int fractionsPerSecond, FractionRoundMode fractionRoundMode)
    {
      bool isFractions = TimeFormat == MediaTimeStringFormat.HHmmssff || TimeFormat == MediaTimeStringFormat.mmssff || TimeFormat == MediaTimeStringFormat.ssff;
      double hours = 0.0, minutes = 0.0, seconds = 0.0, fractions = 0.0;

      if (HoursBox != null && MinutesBox != null && SecondsBox != null && FractionsBox != null)
      {
        _ = double.TryParse(HoursBox.Text, out hours);
        _ = double.TryParse(MinutesBox.Text, out minutes);
        _ = double.TryParse(SecondsBox.Text, out seconds);
        _ = double.TryParse(FractionsBox.Text, out fractions);
      }

      hours *= 3600;
      minutes *= 60;
      fractions = isFractions ? fractions / fractionsPerSecond : fractions / 1000.0;
      seconds += hours + minutes + fractions;

      return MediaTime.FromFractions(TimeSpan.FromSeconds(seconds).Ticks, fractionsPerSecond, fractionRoundMode).Ticks;
    }

    /// <summary>
    /// Notify the change in time.
    /// Notifica el cambio de tiempo.
    /// </summary>
    /// <param name="newTime">
    /// New time.
    /// Nuevo tiempo.
    /// </param>
    /// <param name="oldTime">
    /// Previous time.
    /// Tiempo anterior.
    /// </param>
    private void NotifyTimeChanged(long newTime, long oldTime)
    {
      if (newTime != oldTime)
      {
        var newEventArgs = new MediaTimeTextBoxTimeEventArgs
        {
          NewTime = newTime,
          OldTime = oldTime,
          RoutedEvent = TimeChangedEvent
        };

        RaiseEvent(newEventArgs);
      }
    }

    /// <summary>
    /// Callback for the FillFormat property.
    /// Callback de la propiedad FillFormat.
    /// </summary>
    /// <param name="e">
    /// Event details.
    /// Datos del evento.
    /// </param>
    private void OnFillFormatChanged(DependencyPropertyChangedEventArgs e)
    {
      SetText();
      SetVisualTime();
    }

    /// <summary>
    /// Callback for the FractionsPerSecond property
    /// Callback de la propiedad FractionsPerSecond.
    /// </summary>
    /// <param name="e">
    /// Event details.
    /// Datos del evento.
    /// </param>
    private void OnFractionsPerSecondChanged(DependencyPropertyChangedEventArgs e)
    {
      internalTime = GetVisualTime((int)e.OldValue, FractionRoundMode);
      SetText();
      SetVisualTime();
    }

    /// <summary>
    /// Callback for the FractionRoundMode property.
    /// Callback de la propiedad FractionRoundMode.
    /// </summary>
    /// <param name="e">
    /// Event details.
    /// Datos del evento.
    /// </param>
    private void OnFractionRoundModeChanged(DependencyPropertyChangedEventArgs e)
    {
      internalTime = GetVisualTime(FractionsPerSecond, (FractionRoundMode)e.OldValue);
      SetText();
      SetVisualTime();
    }

    /// <summary>
    /// Callback for the FractionSeparator property.
    /// Callback de la propiedad FractionSeparator.
    /// </summary>
    /// <param name="e">
    /// Event details.
    /// Datos del evento.
    /// </param>
    private void OnFractionSeparatorChanged(DependencyPropertyChangedEventArgs e)
    {
      FractionsSeparator.Text = $"{e.NewValue}";
      SetText();
    }

    /// <summary>
    /// Callback for the Text property.
    /// Callback de la propiedad Text.
    /// </summary>
    /// <param name="e">
    /// Event details.
    /// Datos del evento.
    /// </param>
    private void OnTextChanged(DependencyPropertyChangedEventArgs e)
    {
      if (!this.changingTime)
      {
        this.changingTime = true;

        if (MediaTime.TryParse($"{e.NewValue}", FractionsPerSecond, out MediaTime mts, TimeFormat, FractionRoundMode, FractionSeparator))
          internalTime = mts.Ticks;
        else
          internalTime = 0L;

        SetTime(internalTime);
        changingTime = false;

        if (refreshVisualTime)
          SetVisualTime();
      }
    }

    /// <summary>
    /// Callback for the Time property.
    /// Callback de la propiedad Time.
    /// </summary>
    /// <param name="e">
    /// Event details.
    /// Datos del evento.
    /// </param>
    private void OnTimeChanged(DependencyPropertyChangedEventArgs e)
    {
      if (!changingTime)
      {
        changingTime = true;
        internalTime = (long)e.NewValue;
        SetText();
        changingTime = false;

        if (refreshVisualTime)
            SetVisualTime();
      }

      if (AutoValidateTime)
      {
        if (e.NewValue is long n && e.OldValue is long o)
          NotifyTimeChanged(n, o);
      }
    }

    /// <summary>
    /// Callback for the TimeFormat property.
    /// Callback de la propiedad TimeFormat.
    /// </summary>
    /// <param name="e">
    /// Event details.
    /// Datos del evento.
    /// </param>
    private void OnTimeFormatChanged(DependencyPropertyChangedEventArgs e)
    {
      if (e.NewValue is MediaTimeStringFormat newValue)
      {
        Visibility hoursVisibility = Visibility.Visible;
        Visibility minutesVisibility = Visibility.Visible;
        Visibility secondsVisibility = Visibility.Visible;
        Visibility fractionsVisibility = Visibility.Visible;
        Visibility minutesSeparatorVisibility = Visibility.Visible;
        Visibility secondsSeparatorVisibility = Visibility.Visible;
        Visibility fractionsSeparatorVisibility = Visibility.Visible;
        int hoursMaxLenght = 2;
        int minutesMaxLenght = 2;
        int secondsMaxLenght = 2;
        int fractionsMaxLenght = 2;

        switch (newValue)
        {
          case MediaTimeStringFormat.HHmm:
            fractionsVisibility = Visibility.Collapsed;
            fractionsSeparatorVisibility = Visibility.Collapsed;
            secondsVisibility = Visibility.Collapsed;
            secondsSeparatorVisibility = Visibility.Collapsed;
            break;
          case MediaTimeStringFormat.HHmmss:
            fractionsVisibility = Visibility.Collapsed;
            fractionsSeparatorVisibility = Visibility.Collapsed;
            break;
          case MediaTimeStringFormat.HHmmssff:
            fractionsMaxLenght = MediaTime.FromFractions(0, FractionsPerSecond).DigitPerFraction;
            break;
          case MediaTimeStringFormat.HHmmssmmm:
            fractionsMaxLenght = 3;
            break;
          case MediaTimeStringFormat.mmss:
            minutesMaxLenght = 4;
            fractionsVisibility = Visibility.Collapsed;
            fractionsSeparatorVisibility = Visibility.Collapsed;
            hoursVisibility = Visibility.Collapsed;
            minutesSeparatorVisibility = Visibility.Collapsed;
            break;
          case MediaTimeStringFormat.mmssff:
          case MediaTimeStringFormat.mmssmmm:
            minutesMaxLenght = 4;
            fractionsMaxLenght = newValue == MediaTimeStringFormat.mmssff ? MediaTime.FromFractions(0, FractionsPerSecond).DigitPerFraction : 3;
            hoursVisibility = Visibility.Collapsed;
            minutesSeparatorVisibility = Visibility.Collapsed;
            break;
          case MediaTimeStringFormat.ssff:
          case MediaTimeStringFormat.ssmmm:
            secondsMaxLenght = 6;
            fractionsMaxLenght = newValue == MediaTimeStringFormat.ssff ? MediaTime.FromFractions(0, FractionsPerSecond).DigitPerFraction : 3;
            hoursVisibility = Visibility.Collapsed;
            minutesVisibility = Visibility.Collapsed;
            minutesSeparatorVisibility = Visibility.Collapsed;
            secondsSeparatorVisibility = Visibility.Collapsed;
            break;
        }

        FractionsBox.MaxLength = fractionsMaxLenght;
        FractionsBox.Visibility = fractionsVisibility;
        FractionsSeparator.Visibility = fractionsSeparatorVisibility;

        SecondsBox.MaxLength = secondsMaxLenght;
        SecondsBox.Visibility = secondsVisibility;
        SecondsSeparator.Visibility = secondsSeparatorVisibility;

        MinutesBox.MaxLength = minutesMaxLenght;
        MinutesBox.Visibility = minutesVisibility;
        MinutesSeparator.Visibility = minutesSeparatorVisibility;

        HoursBox.MaxLength = hoursMaxLenght;
        HoursBox.Visibility = hoursVisibility;

        SetText();
        SetVisualTime();
      }
    }

    /// <summary>
    /// Sets the text.
    /// Asigna el texto.
    /// </summary>
    private void SetText() => this.SetCurrentValue(TextProperty, GetTimeString());

    /// <summary>
    /// Sets the time.
    /// Asigna el tiempo.
    /// </summary>
    /// <param name="time">
    /// Time to be set.
    /// Tiempo a asignar.
    /// </param>
    private void SetTime(long time) => SetCurrentValue(TimeProperty, time);

    /// <summary>
    /// Sets the visual values for time.
    /// Asigna los valores visuales del tiempo.
    /// </summary>
    private void SetVisualTime()
    {
      MediaTime mt = MediaTime.FromFractions(this.internalTime, this.FractionsPerSecond, this.FractionRoundMode);
      bool isFractions = TimeFormat == MediaTimeStringFormat.HHmmssff || this.TimeFormat == MediaTimeStringFormat.mmssff || this.TimeFormat == MediaTimeStringFormat.ssff;
      string hoursMaxLength = FillFormat ? $"{HoursBox.MaxLength}" : "2";
      string minutesMaxLength = FillFormat ? $"{MinutesBox.MaxLength}" : "2";
      string secondsMaxLength = FillFormat ? $"{SecondsBox.MaxLength}" : "2";
      string fractionsMaxLength = FillFormat ? $"{FractionsBox.MaxLength}" : $"{mt.DigitPerFraction}";
      
      switch (TimeFormat)
      {
        case MediaTimeStringFormat.HHmm:
        case MediaTimeStringFormat.HHmmss:
        case MediaTimeStringFormat.HHmmssff:
        case MediaTimeStringFormat.HHmmssmmm:
          HoursBox.Text = string.Format("{0:D" + hoursMaxLength + "}", mt.TotalHours);
          MinutesBox.Text = string.Format("{0:D" + minutesMaxLength + "}", mt.Minutes);
          SecondsBox.Text = string.Format("{0:D" + secondsMaxLength + "}", mt.Seconds);

          if (isFractions)
            FractionsBox.Text = string.Format("{0:D" + fractionsMaxLength + "}", mt.Fractions);
          else
            FractionsBox.Text = string.Format("{0:D3}", mt.Milliseconds);
          break;
        case MediaTimeStringFormat.mmss:
        case MediaTimeStringFormat.mmssff:
        case MediaTimeStringFormat.mmssmmm:
          this.HoursBox.Text = string.Format("{0:D" + hoursMaxLength + "}", "0");
          this.MinutesBox.Text = string.Format("{0:D" + minutesMaxLength + "}", mt.TotalMinutes);
          this.SecondsBox.Text = string.Format("{0:D" + secondsMaxLength + "}", mt.Seconds);

          if (isFractions)
            FractionsBox.Text = string.Format("{0:D" + fractionsMaxLength + "}", mt.Fractions);
          else
            FractionsBox.Text = string.Format("{0:D3}", mt.Milliseconds);
          break;
        case MediaTimeStringFormat.ssff:
        case MediaTimeStringFormat.ssmmm:
          this.HoursBox.Text = string.Format("{0:D" + hoursMaxLength + "}", "0");
          this.MinutesBox.Text = string.Format("{0:D" + minutesMaxLength + "}", "0");
          this.SecondsBox.Text = string.Format("{0:D" + secondsMaxLength + "}", mt.TotalSeconds);

          if (isFractions)
            FractionsBox.Text = string.Format("{0:D" + fractionsMaxLength + "}", mt.Fractions);
          else
            FractionsBox.Text = string.Format("{0:D3}", mt.Milliseconds);
          break;
      }
    }

    /// <summary>
    /// TextBox TextChanged event handler.
    /// Controlador del evento TextChanged de TextBox.
    /// </summary>
    /// <param name="sender">
    /// Object that fires the event.
    /// Objeto que desencadena el evento.
    /// </param>
    /// <param name="e">
    /// Event details.
    /// Datos del evento.
    /// </param>
    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
      if (AutoValidateTime)
      {
        long oldTime = internalTime;
        internalTime = GetVisualTime();
        long newTime = internalTime;
        refreshVisualTime = false;
        SetTime(internalTime);
        SetText();
        refreshVisualTime = true;
        NotifyTimeChanged(newTime, oldTime);
      }
    }

    /// <summary>
    /// TextBox PreviewKeyDown event handler.
    /// Controlador del evento PreviewKeyDown de TextBox.
    /// </summary>
    /// <param name="sender">
    /// Object that fires the event.
    /// Objeto que desencadena el evento.
    /// </param>
    /// <param name="e">
    /// Event details.
    /// Datos del evento.
    /// </param>
    private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
    {
      TextBox? box = e.OriginalSource as TextBox ?? e.Source as TextBox;

      if (box != default && e != default && (e.KeyboardDevice.Modifiers & ModifierKeys.Control) != ModifierKeys.Control)
      {
        switch (e.Key)
        {
          case Key.Back:
          case Key.Delete:
          case Key.D0:
          case Key.D1:
          case Key.D2:
          case Key.D3:
          case Key.D4:
          case Key.D5:
          case Key.D6:
          case Key.D7:
          case Key.D8:
          case Key.D9:
          case Key.Enter:
          case Key.NumPad0:
          case Key.NumPad1:
          case Key.NumPad2:
          case Key.NumPad3:
          case Key.NumPad4:
          case Key.NumPad5:
          case Key.NumPad6:
          case Key.NumPad7:
          case Key.NumPad8:
          case Key.NumPad9:
          case Key.Down:
          case Key.Left:
          case Key.Right:
          case Key.Up:
            ActionKey(e);
            break;
          case Key.Escape:
            internalTime = Time;
            SetText();
            SetVisualTime();
            break;
          case Key.Tab:
            box.SelectionStart = 0;
            box.SelectionLength = box.Text.Length;
            break;
          default:
            e.Handled = true;
            break;
        }
      }
    }

    /// <summary>
    /// TextBox PreviewMouseWheel event handler.
    /// Controlador del evento PreviewMouseWheel de TextBox.
    /// </summary>
    /// <param name="sender">
    /// Object that fires the event.
    /// Objeto que desencadena el evento.
    /// </param>
    /// <param name="e">
    /// Event details.
    /// Datos del evento.
    /// </param>
    private void TextBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
      if (!this.IsReadOnly)
      {
        long newTime = 0L;
        long oldTime = 0L;
        TextBox? box = e.OriginalSource as TextBox ?? e.Source as TextBox;

        if (box != null)
        {
          long factor = 0L, value = 1L;

          if (box == this.HoursBox)
            factor = TimeSpan.FromHours(value).Ticks;
          else if (box == this.MinutesBox)
            factor = TimeSpan.FromMinutes(value).Ticks;
          else if (box == this.SecondsBox)
            factor = TimeSpan.FromSeconds(value).Ticks;
          else if (box == this.FractionsBox)
            factor = MediaTime.FromFractions(value, FractionsPerSecond, FractionRoundMode).Ticks;

          if (e.Delta < 0)
          {
            oldTime = internalTime;

            if (internalTime - factor < 0L)
              internalTime = 0L;
            else
              internalTime -= factor;

            newTime = internalTime;

            if (AutoValidateTime)
              NotifyTimeChanged(newTime, oldTime);

            SetVisualTime();
          }
          else
          {
            oldTime = internalTime;
            internalTime += factor;
            newTime = internalTime;

            if (AutoValidateTime)
              NotifyTimeChanged(newTime, oldTime);

            SetVisualTime();
          }
        }
      }
    }
    #endregion
  }
}
