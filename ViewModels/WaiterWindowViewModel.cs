using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using cafeapp1.Models;
using cafeapp1.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

namespace cafeapp1.ViewModels
{
    public partial class WaiterWindowViewModel : ViewModelBase
    {
        private Window _currentWindow;
        private User _currentUser;
        private ObservableCollection<OrderViewModel> _currentShiftOrders;
        private OrderViewModel _selectedOrder;
        private string _errorMessage;
        private string _selectedPaymentMethod;

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

        public string SelectedPaymentMethod
        {
            get => _selectedPaymentMethod;
            set => SetProperty(ref _selectedPaymentMethod, value);
        }

        public WaiterWindowViewModel(Window window, User user)
        {
            _currentWindow = window;
            CurrentUser = user;
            LoadCurrentShiftOrders();
        }

        private void LoadCurrentShiftOrders()
        {
            var currentShift = Service.GetContext().Shifts.FirstOrDefault(s => s.Status == true);
            if (currentShift != null && Service.GetContext().Workersonshifts.Where(w => w.Workerid == CurrentUser.Id && currentShift.Id == w.Shiftid).FirstOrDefault() != null)
            {
                var orders = Service.GetContext().Orders.Include(o => o.StatusNavigation)
                    .Where(o => o.Shiftid == currentShift.Id).ToList();

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
                ErrorMessage = "У вас нет активной смены!";
            }
        }

        public IRelayCommand CreateOrderCommand => new AsyncRelayCommand(CreateOrder);

        public async Task CreateOrder()
        {
            var currentShift = Service.GetContext().Shifts.FirstOrDefault(s => s.Status == true);
            var createOrderWindow = new CreateOrderWindow();
            createOrderWindow.DataContext = new CreateOrderWindowViewModel(createOrderWindow, CurrentUser, currentShift);
            await createOrderWindow.ShowDialog(_currentWindow);
            LoadCurrentShiftOrders();
        }

        public IRelayCommand ChangeOrderStatusCommand => new AsyncRelayCommand(ChangeOrderStatus);

        public async Task ChangeOrderStatus()
        {
            if (SelectedOrder != null)
            {
                var changeOrderStatusWindow = new ChangeOrderStatusWindow();
                changeOrderStatusWindow.DataContext = new ChangeOrderStatusWindowViewModel(changeOrderStatusWindow, SelectedOrder.Order, CurrentUser);
                await changeOrderStatusWindow.ShowDialog(_currentWindow);
                LoadCurrentShiftOrders();
            }
        }

        public ObservableCollection<string> PaymentMethods { get; } = new ObservableCollection<string> { "Наличная", "Безналичная" };
        public IRelayCommand GenerateExcelReceiptCommand => new RelayCommand(GenerateExcelReceipt);

        private void GenerateExcelReceipt()
        {
            if (SelectedOrder != null && !string.IsNullOrEmpty(SelectedPaymentMethod))
            {
                var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "/../../../Assets/reports/");
                filePath += DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    var worksheet = package.Workbook.Worksheets.Add("Receipt");

                    worksheet.Cells[1, 1].Value = "Номер заказа";
                    worksheet.Cells[1, 2].Value = "Номер стола";
                    worksheet.Cells[1, 3].Value = "Блюда";
                    worksheet.Cells[1, 4].Value = "Цена";
                    worksheet.Cells[1, 5].Value = "Способ оплаты";

                    worksheet.Cells[2, 1].Value = SelectedOrder.Order.Id;
                    worksheet.Cells[2, 2].Value = SelectedOrder.Order.Tableid;
                    worksheet.Cells[2, 5].Value = SelectedPaymentMethod;

                    var foods = Service.GetContext().Foodonorders
                        .Where(fo => fo.Idorder == SelectedOrder.Order.Id)
                        .Select(fo => new { fo.IdfoodNavigation.Name, fo.IdfoodNavigation.Price })
                        .ToList();

                    int row = 3;
                    double total = 0;
                    foreach (var food in foods)
                    {
                        worksheet.Cells[row, 3].Value = food.Name;
                        worksheet.Cells[row, 4].Value = food.Price;
                        total += (double)food.Price;
                        row++;
                    }

                    worksheet.Cells[row, 3].Value = "Итого";
                    worksheet.Cells[row, 4].Value = total;

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
    
    

    public class OrderViewModel : ObservableObject
    {
        private Order _order;
        private string _foodsInOrder;

        public Order Order
        {
            get => _order;
            set => SetProperty(ref _order, value);
        }

        public string FoodsInOrder
        {
            get => _foodsInOrder;
            set => SetProperty(ref _foodsInOrder, value);
        }
    }
}
