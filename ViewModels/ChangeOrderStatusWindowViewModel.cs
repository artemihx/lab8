using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using cafeapp1.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace cafeapp1.ViewModels
{
    public partial class ChangeOrderStatusWindowViewModel : ViewModelBase
    {
        private Window _currentWindow;
        private User _currentUser;
        private Order _order;
        private ObservableCollection<Orderstatus> _orderStatuses;

        public User CurrentUser
        {
            get => _currentUser;
            set => SetProperty(ref _currentUser, value);
        }
        public Order Order
        {
            get => _order;
            set => SetProperty(ref _order, value);
        }

        public ObservableCollection<Orderstatus> OrderStatuses
        {
            get => _orderStatuses;
            set => SetProperty(ref _orderStatuses, value);
        }

        public ChangeOrderStatusWindowViewModel(Window window, Order order, User user)
        {
            _currentUser = user;
            _currentWindow = window;
            Order = order;
            LoadOrderStatuses();
        }

        private void LoadOrderStatuses()
        {
            var allStatuses = Service.GetContext().Orderstatuses.ToList();

            var filteredStatuses = allStatuses.ToList();
            if (CurrentUser != null && CurrentUser.Roleid == 2)
            {
                filteredStatuses = allStatuses.Where(s => s.Name != "Готов" && s.Name != "Готовится").ToList();
            }
            else if (CurrentUser != null && CurrentUser.Roleid == 3)
            {
                filteredStatuses = allStatuses.Where(s => s.Name != "Ожидает" && s.Name != "Отменен" && s.Name != "Оплачен").ToList();
            }
            else if (CurrentUser != null && CurrentUser.Roleid == 1)
            {
                filteredStatuses = allStatuses.ToList();
            }
            OrderStatuses = new ObservableCollection<Orderstatus>(filteredStatuses);
        }

        public IRelayCommand SaveOrderCommand => new RelayCommand(SaveOrder);

        private void SaveOrder()
        {
            Service.GetContext().Orders.Update(Order);
            Service.GetContext().SaveChanges();
            _currentWindow.Close();
        }
    }
}