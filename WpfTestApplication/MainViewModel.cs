using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace WpfTestApplication
{
  public class MainViewModel : INotifyPropertyChanged
  {
    #region Events
    public event PropertyChangedEventHandler? PropertyChanged;
    #endregion

    #region Commands
    public RelayCommand<object> AcceptCommand => new(ExecuteAccept, i => CanAccept());
    #endregion

    #region Properties
    private bool isValidMandatoryText;
    public bool IsValidMandatoryText
    {
      get => isValidMandatoryText;
      set { isValidMandatoryText = value; NotifyPropertyChanged(); }
    }

    private bool isValidMandatoryShort;
    public bool IsValidMandatoryShort
    {
      get => isValidMandatoryShort;
      set { isValidMandatoryShort = value; NotifyPropertyChanged(); }
    }

    private bool isValidOptionalShort;
    public bool IsValidOptionalShort
    {
      get => isValidOptionalShort;
      set { isValidOptionalShort = value; NotifyPropertyChanged(); }
    }

    private bool isValidMandatoryPercentage;
    public bool IsValidMandatoryPercentage
    {
      get => isValidMandatoryPercentage;
      set { isValidMandatoryPercentage = value; NotifyPropertyChanged(); }
    }

    private bool isValidMandatoryDateFrom;
    public bool IsValidMandatoryDateFrom
    {
      get => isValidMandatoryDateFrom;
      set { isValidMandatoryDateFrom = value; NotifyPropertyChanged(); }
    }
    
    private bool isValidOptionalDateTo;
    public bool IsValidOptionalDateTo
    {
      get => isValidOptionalDateTo;
      set { isValidOptionalDateTo = value; NotifyPropertyChanged(); }
    }

    private bool isValidOptionalEmail;
    public bool IsValidOptionalEmail
    {
      get => isValidOptionalEmail;
      set { isValidOptionalEmail = value; NotifyPropertyChanged(); }
    }

    private bool isValidBetween1and1;
    public bool IsValidBetween1and1
    {
      get => isValidBetween1and1;
      set { isValidBetween1and1 = value; NotifyPropertyChanged(); }
    }

    private bool isValidWebSitePattern;
    public bool IsValidWebSitePattern
    {
      get => isValidWebSitePattern;
      set { isValidWebSitePattern = value; NotifyPropertyChanged(); }
    }

    private bool isValidEmailPattern;
    public bool IsValidEmailPattern
    {
      get => isValidEmailPattern;
      set { isValidEmailPattern = value; NotifyPropertyChanged(); }
    }

    private string emailPattern = @"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?";
    public string EmailPattern
    {
      get => emailPattern;
      set { emailPattern = value; NotifyPropertyChanged(); }
    }

    private string webSitePattern = @"^http(s)?://([\w-]+.)+[\w-]+(/[\w- ./?%&=])?$";
    public string WebSitePattern
    {
      get => webSitePattern;
      set { webSitePattern = value; NotifyPropertyChanged(); }
    }

    private Entity? entity;
    public Entity? Entity
    {
      get { return entity; }
      set { entity = value; NotifyPropertyChanged(); }
    }

    #endregion

    #region Private methods
    private bool CanAccept()
    {
      return IsValidMandatoryText 
        && IsValidMandatoryShort 
        && IsValidOptionalShort 
        && IsValidMandatoryPercentage 
        && IsValidMandatoryDateFrom 
        && IsValidOptionalDateTo 
        && IsValidOptionalEmail 
        && IsValidBetween1and1 
        && IsValidWebSitePattern 
        && IsValidEmailPattern;
    }

    private void ExecuteAccept(object param)
    {
      Debug.WriteLine("Accepted");
      System.Windows.MessageBox.Show("Accepted");
    }
    private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion
  }
}
