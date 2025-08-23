namespace Espamatica.Core
{
  /// <summary>
  /// Fraction rounding mode.
  /// Modo de redondeo de fracciones.
  /// </summary>
  /// <see cref="https://espamatica.com/mediatime/"/>
  public enum FractionRoundMode
  {
    /// <summary>
    /// The fraction is truncated.
    /// La fracción de trunca.
    /// </summary>
    Truncate,

    /// <summary>
    /// If the fraction has decimals, it is truncated and 1 is added to it.
    /// Si la fracción tiene decimales, se trunca y se le suma 1.
    /// </summary>
    Real,

    /// <summary>
    /// The fraction is rounded.
    /// La fracción se redondea.
    /// </summary>
    Round
  }
}
