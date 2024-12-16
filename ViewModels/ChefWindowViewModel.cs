using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using cafeapp1.Models;
using cafeapp1.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;

namespace cafeapp1.ViewModels
{
    public partial class ChefWindowViewModel : ViewModelBase
    {
        private Window _currentWindow;
        private User _currentUser;
        private ObservableCollection<OrderViewModel> _currentShiftOrders;
        private OrderViewModel _selectedOrder;
        private string _errorMessage;

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

        public ChefWindowViewModel(Window window, User user)
        {
            _currentWindow = window;
            CurrentUser = user;
            LoadCurrentShiftOrders();
        }

        private void LoadCurrentShiftOrders()
        {
            var currentShift = Service.GetContext().Shifts.FirstOrDefault(s => s.Status == true);
            if (currentShift != null && currentShift.Workersonshifts.Where(w => w.Workerid == CurrentUser.Id) != null)
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
    }
}
