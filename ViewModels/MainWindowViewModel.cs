using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using cafeapp1.Models;
using cafeapp1.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;

namespace cafeapp1.ViewModels;

public partial class MainWindowViewModel() : ViewModelBase
{
    private Window _currentWindow;
    private string _username;
    private string _password;
    private string _errorMessage;
    public string Username
    {
        get => _username;
        set => SetProperty(ref _username, value);
    }

    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public IRelayCommand LoginCommand { get; }

    public MainWindowViewModel(Window currentWindow) : this()
    {
        LoginCommand = new RelayCommand(Login);
        _currentWindow = currentWindow;
    }

    private void Login()
    {
        var user = Service.GetContext().Users.Where(u => u.Login == Username).FirstOrDefault();
        if (user != null)
        {
            if (user.Password == Password)
            { 
                if (user.Roleid == 1)
                {
                    var adminWindow = new AdminWindow();
                    adminWindow.DataContext = new AdminWindowViewModel(adminWindow, user);
                    adminWindow.Show();
                    _currentWindow.Close();
                }

                if (user.Roleid == 2)
                {
                    var waiterWindow = new WaiterWindow();
                    waiterWindow.DataContext = new WaiterWindowViewModel(waiterWindow, user);
                    waiterWindow.Show();
                    _currentWindow.Close();
                }

                if (user.Roleid == 3)
                {
                    var chefWindow = new ChefWindow();
                    chefWindow.DataContext = new ChefWindowViewModel(chefWindow, user);
                    chefWindow.Show();
                    _currentWindow.Close();
                }
            }
            else
            {
                ErrorMessage = "Неправильный пароль";
            }
        }
        else
        {
            ErrorMessage = "Пользователь не найден";
        }
    }
    
    
}
