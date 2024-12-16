using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using cafeapp1.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace cafeapp1.ViewModels
{
    public partial class EmployeeWindowViewModel : ViewModelBase
    {
        private Window _currentWindow;
        private User _selectedEmployee;
        private ObservableCollection<Role> _roles;
        private Role _selectedRole;

        public User SelectedEmployee
        {
            get => _selectedEmployee;
            set => SetProperty(ref _selectedEmployee, value);
        }

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
                if (SelectedEmployee != null)
                {
                    SelectedEmployee.Roleid = value.Id;
                }
            }
        }

        public EmployeeWindowViewModel(Window window, User user)
        {
            _currentWindow = window;
            SelectedEmployee = user;
            Roles = new ObservableCollection<Role>(Service.GetContext().Roles.ToList());
            SelectedRole = Roles.FirstOrDefault(r => r.Id == SelectedEmployee.Roleid);
        }
        
        public IRelayCommand SaveCommand => new RelayCommand(Save);

        private void Save()
        {
            Service.GetContext().Users.Update(SelectedEmployee);
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
