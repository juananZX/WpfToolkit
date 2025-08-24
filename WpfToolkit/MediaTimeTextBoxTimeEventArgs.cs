using System.Windows;

namespace Espamatica.WpfToolkit
{
  public class MediaTimeTextBoxTimeEventArgs : RoutedEventArgs
  {
    #region Properties
    public long NewTime { get; set; }

    public long OldTime { get; set; }
    #endregion
  }
}
