using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

        public AsyncRelayCommand UploadPhotoCommand => new AsyncRelayCommand(UploadPhoto);

        private async Task UploadPhoto()
        {
            var dialog = new OpenFileDialog();
            dialog.Filters.Add(new FileDialogFilter() { Name = "Images", Extensions = { "jpg", "png" } });

            var result = await dialog.ShowAsync(_currentWindow);
            if (result != null && result.Length > 0)
            {
                var selectedFile = result[0];
                var fileName = Path.GetFileName(selectedFile);
                var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                var directoryPath = Path.Combine(baseDirectory + "/../../../Assets/users-photo/");
                var destinationPath = Path.Combine(directoryPath, fileName);
                await Task.Run(() => File.Copy(selectedFile, destinationPath, true));
                await Service.WaitForFileToAppear(destinationPath);
                SelectedEmployee.Photo = fileName;
                Service.GetContext().Users.Update(SelectedEmployee);
                await Task.Run((() => Service.GetContext().SaveChanges()));
            }
        }
    }
}