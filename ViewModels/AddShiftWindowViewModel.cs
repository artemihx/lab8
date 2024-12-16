using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using cafeapp1.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace cafeapp1.ViewModels
{
    public partial class AddShiftWindowViewModel : ViewModelBase
    {
        private Window _currentWindow;
        private DateTimeOffset _shiftDate;
        private ObservableCollection<User> _allEmployees;
        private ObservableCollection<User> _employeesOnShift;
        private User _selectedEmployeeToAdd;
        private User _selectedEmployeeToRemove;

        public DateTimeOffset ShiftDate
        {
            get => _shiftDate;
            set => SetProperty(ref _shiftDate, value);
        }

        public ObservableCollection<User> AllEmployees
        {
            get => _allEmployees;
            set => SetProperty(ref _allEmployees, value);
        }

        public ObservableCollection<User> EmployeesOnShift
        {
            get => _employeesOnShift;
            set => SetProperty(ref _employeesOnShift, value);
        }

        public User SelectedEmployeeToAdd
        {
            get => _selectedEmployeeToAdd;
            set => SetProperty(ref _selectedEmployeeToAdd, value);
        }

        public User SelectedEmployeeToRemove
        {
            get => _selectedEmployeeToRemove;
            set => SetProperty(ref _selectedEmployeeToRemove, value);
        }

        public AddShiftWindowViewModel(Window window)
        {
            _currentWindow = window;
            ShiftDate = new DateTimeOffset(DateTime.Now);
            AllEmployees = new ObservableCollection<User>(Service.GetContext().Users.Where(u => u.Status == true).ToList());
            EmployeesOnShift = new ObservableCollection<User>();
        }

        public IRelayCommand AddEmployeeCommand => new RelayCommand(AddEmployee);
        private void AddEmployee()
        {
            if (SelectedEmployeeToAdd != null)
            {
                EmployeesOnShift.Add(SelectedEmployeeToAdd);
                AllEmployees.Remove(SelectedEmployeeToAdd);
                SelectedEmployeeToAdd = null;
            }
        }

        public IRelayCommand RemoveEmployeeCommand => new RelayCommand(RemoveEmployee);
        private void RemoveEmployee()
        {
            if (SelectedEmployeeToRemove != null)
            {
                AllEmployees.Add(SelectedEmployeeToRemove);
                EmployeesOnShift.Remove(SelectedEmployeeToRemove);
                SelectedEmployeeToRemove = null;
            }
        }

        public IRelayCommand SaveShiftCommand => new AsyncRelayCommand(SaveShift);

        public async Task SaveShift()
        {
            var newShift = new Shift
            {
                Date = ShiftDate.DateTime,
                Status = false
            };
            Service.GetContext().Shifts.Add(newShift);
            Service.GetContext().SaveChanges();

            // Добавляем новые записи в таблицу Workersonshift
            foreach (var employee in EmployeesOnShift)
            {
                var workersonshift = new Workersonshift
                {
                    Shiftid = newShift.Id,
                    Workerid = employee.Id
                };
                Service.GetContext().Workersonshifts.Add(workersonshift);
            }

            Service.GetContext().SaveChanges();

            _currentWindow.Close();
        }
    }
}
