using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using cafeapp1.Models;
using cafeapp1.Views;
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
            NewUser.Id = Service.GetContext().Users.Max(x => x.Id) + 1;
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
                NewUser.Photo = fileName;
                Service.GetContext().Users.Update(NewUser);
                await Task.Run((() => Service.GetContext().SaveChanges()));
            }
        }
    }
}
