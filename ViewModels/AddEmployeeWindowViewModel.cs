using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using cafeapp1.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace cafeapp1.ViewModels
{
    public partial class AddEmployeeWindowViewModel : ViewModelBase
    {
        private Window _currentWindow;
        private User _newUser;
        private ObservableCollection<Role> _roles;
        private Role _selectedRole;
        public ObservableCollection<Role> Roles
        {
            get => _roles;
            set => SetProperty(ref _roles, value);
        }

        public Role SelectedRole
        {
            get => _selectedRole;
            set
            {
                SetProperty(ref _selectedRole, value);
                NewUser.Role = _selectedRole;
            }
        }

        public User NewUser
        {
            get => _newUser;
            set => SetProperty(ref _newUser, value);
        }

        public AddEmployeeWindowViewModel(Window window)
        {
            NewUser = new User();
            _currentWindow = window;
            Roles = new ObservableCollection<Role>(Service.GetContext().Roles.ToList());;
        }
        
        public IRelayCommand SaveCommand => new RelayCommand(Save);

        private void Save()
        {
            Service.GetContext().Users.Add(NewUser);
            Service.GetContext().SaveChanges();
            _currentWindow.Close();
        }

        public IRelayCommand UploadPhotoCommand => new RelayCommand(UploadPhoto);

        private void UploadPhoto()
        {
            // Логика для загрузки фотографии
        }
    }
}
