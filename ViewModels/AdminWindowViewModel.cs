using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using cafeapp1.Models;
using cafeapp1.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileSystemGlobbing.Internal.PathSegments;
using OfficeOpenXml;

namespace cafeapp1.ViewModels
{
    public partial class AdminWindowViewModel : ViewModelBase
    {
        private Window _currentWindow;
        private User _currentUser;
        private ObservableCollection<User> _employees;
        private User _selectedEmployee;
        private ObservableCollection<Shift> _shifts;
        private Shift _selectedShift;
        private Shift _currentShift;
        private ObservableCollection<OrderViewModel> _currentShiftOrders;
        private OrderViewModel _selectedOrder;
        private string _errorMessage;
        private string _photo;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }
        public User CurrentUser
        {
            get => _currentUser;
            set => SetProperty(ref _currentUser, value);
        }

        public ObservableCollection<User> Employees
        {
            get => _employees;
            set => SetProperty(ref _employees, value);
        }

        public User SelectedEmployee
        {
            get => _selectedEmployee;
            set => SetProperty(ref _selectedEmployee, value);
        }

        public ObservableCollection<Shift> Shifts
        {
            get => _shifts;
            set => SetProperty(ref _shifts, value);
        }

        public Shift SelectedShift
        {
            get => _selectedShift;
            set => SetProperty(ref _selectedShift, value);
        }

        public Shift CurrentShift
        {
            get => _currentShift;
            set => SetProperty(ref _currentShift, value);
        }

        public ObservableCollection<OrderViewModel> CurrentShiftOrders
        {
            get => _currentShiftOrders;
            set => SetProperty(ref _currentShiftOrders, value);
        }

        public OrderViewModel SelectedOrder
        {
            get => _selectedOrder;
            set => SetProperty(ref _selectedOrder, value);
        }

        public AdminWindowViewModel(Window window, User user)
        {
            _currentWindow = window;
            CurrentUser = user;
            Employees = new ObservableCollection<User>(
                new ObservableCollection<User>(Service.GetContext().Users.Include(u => u.Role)
                    .Where(u => u != CurrentUser).ToList()));
            Shifts = new ObservableCollection<Shift>(Service.GetContext().Shifts.ToList());
            CurrentShift = Shifts.FirstOrDefault(s => s.Status == true);
            LoadCurrentShiftOrders();
        }

        private void LoadCurrentShiftOrders()
        {
            if (CurrentShift != null)
            {
                var orders = Service.GetContext().Orders.Include(o => o.StatusNavigation)
                    .Where(o => o.Shiftid == CurrentShift.Id).ToList();

                var orderViewModels = orders.Select(o => new OrderViewModel
                {
                    Order = o,
                    FoodsInOrder = string.Join(", ", Service.GetContext().Foodonorders
                        .Where(fo => fo.Idorder == o.Id)
                        .Select(fo => fo.IdfoodNavigation.Name))
                }).ToList();

                CurrentShiftOrders = new ObservableCollection<OrderViewModel>(orderViewModels);
            }
            else
            {
                ErrorMessage = "Нет активной смены!";
            }
        }

        public IRelayCommand SaveUserCommand => new RelayCommand(SaveUser);

        private void SaveUser()
        {
            Service.GetContext().SaveChanges();
        }

        public IRelayCommand SaveEmployeeCommand => new RelayCommand(SaveEmployee);

        private void SaveEmployee()
        {
            Service.GetContext().SaveChanges();
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
                File.Copy(selectedFile, destinationPath, true);

                _photo = fileName;
                CurrentUser.Photo = _photo;
            }
        }

        public IRelayCommand EditDataCommand => new RelayCommand(EditData);

        private void EditData()
        {
            Service.GetContext().Users.Update(CurrentUser);
            Service.GetContext().SaveChanges();
            CurrentUser = Service.GetContext().Users.Find(CurrentUser.Id);
        }

        public IRelayCommand EditEmployeeCommand => new AsyncRelayCommand(EditEmployee);

        public async Task EditEmployee()
        {
            if (SelectedEmployee != null)
            {
                var emlpoyeeWindow = new EmployeeWindow();
                emlpoyeeWindow.DataContext = new EmployeeWindowViewModel(emlpoyeeWindow, SelectedEmployee);
                await emlpoyeeWindow.ShowDialog(_currentWindow);
                Employees = new ObservableCollection<User>(Service.GetContext().Users.Where(u => u != CurrentUser).ToList());
            }
        }

        public IRelayCommand AddCommand => new AsyncRelayCommand(AddEmployee);

        public async Task AddEmployee()
        {
            var addEmployeeWindow = new AddEmployeeWindow();
            addEmployeeWindow.DataContext = new AddEmployeeWindowViewModel(addEmployeeWindow);
            await addEmployeeWindow.ShowDialog(_currentWindow);
            Employees = new ObservableCollection<User>(Service.GetContext().Users.Where(u => u != CurrentUser).ToList());
        }

        public IRelayCommand DeleteEmployeeCommand => new AsyncRelayCommand(DeleteEmployee);

        public async Task DeleteEmployee()
        {
            if (SelectedEmployee != null)
            {
                Service.GetContext().Users.Remove(SelectedEmployee);
                Service.GetContext().SaveChanges();
                Employees = new ObservableCollection<User>(Service.GetContext().Users.Where(u => u != CurrentUser).ToList());
            }
        }

        public IRelayCommand FireOrAcceptCommand => new AsyncRelayCommand(FireOrAccept);

        public async Task FireOrAccept()
        {
            if (SelectedEmployee != null)
            {
                if (SelectedEmployee.Status == true)
                {
                    SelectedEmployee.Status = false;
                }
                else
                {
                    SelectedEmployee.Status = true;
                }
                Service.GetContext().Users.Update(SelectedEmployee);
                Service.GetContext().SaveChanges();
                Employees = new ObservableCollection<User>(Service.GetContext().Users.Where(u => u != CurrentUser).ToList());
            }
        }

        public IRelayCommand ToggleShiftCommand => new RelayCommand(ToggleShift);

        private void ToggleShift()
        {
            if (SelectedShift != null)
            {
                if (SelectedShift.Status)
                {
                    SelectedShift.Status = false;
                }
                else
                {
                    if (Shifts.Any(s => s.Status == true))
                    {
                        ErrorMessage = "Уже есть открытая смена!";
                        return;
                    }
                    if (SelectedShift.Date < DateTime.Today)
                    {
                        ErrorMessage = "Смена не должна быть раньше текущей даты!";
                        return;
                    }
                    var workersOnShift = Service.GetContext().Workersonshifts.Where(w => w.Shiftid == SelectedShift.Id).ToList();
                    if (workersOnShift.Count < 2)
                    {
                        ErrorMessage = "Недостаточно сотрудников!";
                        return;
                    }

                    ErrorMessage = "";
                    SelectedShift.Status = true;
                }
                Service.GetContext().Shifts.Update(SelectedShift);
                Service.GetContext().SaveChanges();
                Shifts = new ObservableCollection<Shift>(Service.GetContext().Shifts.ToList());
                CurrentShift = Shifts.FirstOrDefault(s => s.Status == true);
                LoadCurrentShiftOrders();
            }
        }

        public IRelayCommand AddShiftCommand => new AsyncRelayCommand(AddShift);

        public async Task AddShift()
        {
            var addShiftWindow = new AddShiftWindow();
            addShiftWindow.DataContext = new AddShiftWindowViewModel(addShiftWindow);
            await addShiftWindow.ShowDialog(_currentWindow);
            Shifts = new ObservableCollection<Shift>(Service.GetContext().Shifts.ToList());
            CurrentShift = Shifts.FirstOrDefault(s => s.Status == true);
            LoadCurrentShiftOrders();
        }

        public IRelayCommand EditShiftCommand => new AsyncRelayCommand(EditShift);

        public async Task EditShift()
        {
            if (SelectedShift != null)
            {
                var editShiftWindow = new EditShiftWindow();
                editShiftWindow.DataContext = new EditShiftWindowViewModel(editShiftWindow, SelectedShift);
                await editShiftWindow.ShowDialog(_currentWindow);
                Shifts = new ObservableCollection<Shift>(Service.GetContext().Shifts.ToList());
                CurrentShift = Shifts.FirstOrDefault(s => s.Status == true);
                LoadCurrentShiftOrders();
            }
        }

        public IRelayCommand DeleteShiftCommand => new AsyncRelayCommand(DeleteShift);

        public async Task DeleteShift()
        {
            if (SelectedShift != null)
            {
                var workersonshifts = Service.GetContext().Workersonshifts.Where(w => w.Shiftid == SelectedShift.Id).ToList();
                Service.GetContext().Workersonshifts.RemoveRange(workersonshifts);

                Service.GetContext().Shifts.Remove(SelectedShift);
                Service.GetContext().SaveChanges();

                Shifts = new ObservableCollection<Shift>(Service.GetContext().Shifts.ToList());
                CurrentShift = Shifts.FirstOrDefault(s => s.Status == true);
                LoadCurrentShiftOrders();
            }
        }

        public IRelayCommand AssignTablesCommand => new AsyncRelayCommand(AssignTables);

        public async Task AssignTables()
        {
            var assignTablesWindow = new AssignTablesWindow();
            assignTablesWindow.DataContext = new AssignTablesWindowViewModel(assignTablesWindow);
            await assignTablesWindow.ShowDialog(_currentWindow);
        }

        public IRelayCommand EditOrderCommand => new AsyncRelayCommand(EditOrder);

        public async Task EditOrder()
        {
            if (SelectedOrder != null && SelectedOrder.Order.StatusNavigation.Name != "Оплачен")
            {
                var editOrderWindow = new EditOrderWindow();
                editOrderWindow.DataContext = new EditOrderWindowViewModel(editOrderWindow,CurrentUser,SelectedOrder.Order);
                await editOrderWindow.ShowDialog(_currentWindow);
                LoadCurrentShiftOrders();
            }
        }

        public IRelayCommand ChangeOrderStatusCommand => new AsyncRelayCommand(ChangeOrderStatus);

        public async Task ChangeOrderStatus()
        {
            if (SelectedOrder != null)
            {
                var changeOrderStatusWindow = new ChangeOrderStatusWindow();
                changeOrderStatusWindow.DataContext =
                    new ChangeOrderStatusWindowViewModel(changeOrderStatusWindow, SelectedOrder.Order, CurrentUser);
                await changeOrderStatusWindow.ShowDialog(_currentWindow);
                LoadCurrentShiftOrders();
            }
        }

        public IRelayCommand GenerateShiftReportCommand => new RelayCommand(GenerateShiftReport);

        private void GenerateShiftReport()
        {
            if (CurrentShift != null)
            {
                var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "/../../../Assets/reports/");
                filePath +=  "admin" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    var worksheet = package.Workbook.Worksheets.Add("ShiftReport");

                    worksheet.Cells[1, 1].Value = "Номер заказа";
                    worksheet.Cells[1, 2].Value = "Номер стола";
                    worksheet.Cells[1, 3].Value = "Блюда";
                    worksheet.Cells[1, 4].Value = "Цена";

                    var orders = Service.GetContext().Orders.Include(o => o.StatusNavigation)
                        .Where(o => o.Shiftid == CurrentShift.Id).ToList();

                    int row = 2;
                    double totalSum = 0;

                    foreach (var order in orders)
                    {
                        worksheet.Cells[row, 1].Value = order.Id;
                        worksheet.Cells[row, 2].Value = order.Tableid;

                        var foods = Service.GetContext().Foodonorders
                            .Where(fo => fo.Idorder == order.Id)
                            .Select(fo => new { fo.IdfoodNavigation.Name, fo.IdfoodNavigation.Price })
                            .ToList();

                        double orderTotal = 0;
                        foreach (var food in foods)
                        {
                            worksheet.Cells[row, 3].Value = food.Name;
                            worksheet.Cells[row, 4].Value = food.Price;
                            orderTotal += (double)food.Price;
                            row++;
                        }

                        worksheet.Cells[row, 3].Value = "Итого за заказ";
                        worksheet.Cells[row, 4].Value = orderTotal;
                        totalSum += orderTotal;
                        row++;
                    }

                    worksheet.Cells[row, 3].Value = "Общая сумма за смену";
                    worksheet.Cells[row, 4].Value = totalSum;

                    package.Save();

                    OpenFile(filePath);
                }
            }
        }

        private void OpenFile(string filePath)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true
                }
            };
            process.Start();
        }

    }
}
