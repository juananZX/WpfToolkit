using System.Windows;
using System.Windows.Controls;

namespace Espamatica.WpfToolkit
{
  public class DataGridSummary : ScrollViewer
  {
    #region Attached dependency properties
    public readonly static DependencyProperty DoSummaryProperty = 
      DependencyProperty.RegisterAttached("DoSummary", typeof(bool), typeof(DataGridColumn), new PropertyMetadata(false));
    public static bool GetDoSummary(DataGridColumn obj) => (bool)obj.GetValue(DoSummaryProperty);
    public static void SetDoSummary(DataGridColumn obj, bool value) => obj.SetValue(DoSummaryProperty, value);

    public readonly static DependencyProperty ExcludeFromSummaryProperty =
      DependencyProperty.RegisterAttached("ExcludeFromSummary", typeof(bool), typeof(DataGridColumn), new PropertyMetadata(false));
    public static bool GetExcludeFromSummary(DataGridColumn obj) => (bool)obj.GetValue(ExcludeFromSummaryProperty);
    public static void SetExcludeFromSummary(DataGridColumn obj, bool value) => obj.SetValue(ExcludeFromSummaryProperty, value);

    public readonly static DependencyProperty SummaryFormatProperty =
     DependencyProperty.RegisterAttached("SummaryFormat", typeof(string), typeof(DataGridColumn), new PropertyMetadata(string.Empty));
    public static string GetSummaryFormat(DataGridColumn obj) => $"{obj.GetValue(SummaryFormatProperty)}";
    public static void SetSummaryFormat(DataGridColumn obj, string value) => obj.SetValue(SummaryFormatProperty, value);

    public readonly static DependencyProperty SummaryPrefixProperty =
      DependencyProperty.RegisterAttached("SummaryPrefix", typeof(string), typeof(DataGridColumn), new PropertyMetadata(string.Empty));
    public static string GetSummaryPrefix(DataGridColumn obj) => $"{obj.GetValue(SummaryPrefixProperty)}";
    public static void SetSummaryPrefix(DataGridColumn obj, string value) => obj.SetValue(SummaryPrefixProperty , value);

    public readonly static DependencyProperty SummarySufixProperty =
      DependencyProperty.RegisterAttached("SummarySufix", typeof(string), typeof(DataGridColumn), new PropertyMetadata(string.Empty));
    public static string GetSummarySufix(DataGridColumn obj) => $"{obj.GetValue(SummarySufixProperty)}";
    public static void SetSummarySufix(DataGridColumn obj, string value) => obj.SetValue(SummarySufixProperty, value);

    public readonly static DependencyProperty SummaryTypeProperty =
      DependencyProperty.RegisterAttached("SummaryType", typeof(SummaryType), typeof(DataGridColumn), new PropertyMetadata(SummaryType.Sum));
    public static SummaryType GetSummaryType(DataGridColumn obj) => (SummaryType)obj.GetValue(SummaryTypeProperty);
    public static void SetSummaryType(DataGridColumn obj, SummaryType value) => obj.SetValue(SummaryTypeProperty, value);
    #endregion

    #region Dependency properties
    public static readonly DependencyProperty OwnerDataGridProperty = DependencyProperty.Register(nameof(OwnerDataGrid), typeof(DataGrid), typeof(DataGridSummary), new PropertyMetadata(new PropertyChangedCallback(OnOwnerDataGridChanged)));

    public DataGrid? OwnerDataGrid 
    { 
      get => (DataGrid?)GetValue(OwnerDataGridProperty);
      set => SetValue(OwnerDataGridProperty, value);
    }

    private static void OnOwnerDataGridChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      DataGridSummary summaryDataGrid = d as DataGridSummary ?? throw new InvalidCastException("The dependency object is not a SummaryDataGrid");
      summaryDataGrid.OnOwnerDataGridChanged(e);
    }

    private void OnOwnerDataGridChanged(DependencyPropertyChangedEventArgs e)
    {
      if (e.OldValue is DataGrid oldDataGrid)
      {
        oldDataGrid.ColumnReordered -= OwnerDataGrid_ColumnReordered;
        oldDataGrid.LayoutUpdated -= OwnerDataGrid_LayoutUpdated;
        oldDataGrid.Loaded -= OwnerDataGrid_Loaded;
      }

      if (e.NewValue is DataGrid newDataGrid)
      {
        newDataGrid.ColumnReordered += OwnerDataGrid_ColumnReordered;
        newDataGrid.LayoutUpdated += OwnerDataGrid_LayoutUpdated;
        newDataGrid.Loaded += OwnerDataGrid_Loaded;
      }
    }
    #endregion

    #region Fields
    private readonly StackPanel panel;
    #endregion

    #region Constructors
    public DataGridSummary()
    {
      HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
      VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;

      panel = new StackPanel { Orientation = Orientation.Horizontal };
      AddChild(panel);
    }
    #endregion

    #region Listeners
    private void OwnerDataGrid_ColumnReordered(object? sender, DataGridColumnEventArgs e)
    {
      
    }

    private void OwnerDataGrid_LayoutUpdated(object? sender, EventArgs e)
    {

    }

    private void OwnerDataGrid_Loaded(object sender, RoutedEventArgs e)
    {
      
    }    
    #endregion
  }
}
