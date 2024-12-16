using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using cafeapp1.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace cafeapp1.ViewModels
{
    public partial class CreateOrderWindowViewModel : ViewModelBase
    {
        private Window _currentWindow;
        private Order _order;
        private ObservableCollection<Orderstatus> _orderStatuses;
        private ObservableCollection<Table> _tables;
        private User _currentUser;
        private ObservableCollection<Food> _allFoods;
        private ObservableCollection<Food> _foodsInOrder;
        private Food _selectedFoodToAdd;
        private Food _selectedFoodToRemove;
        private bool _isOrderSaved;
        private Shift _currentShift;

        public Shift CurrentShift
        {
            get => _currentShift;
            set => SetProperty(ref _currentShift, value);
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

        public ObservableCollection<Table> Tables
        {
            get => _tables;
            set => SetProperty(ref _tables, value);
        }

        public ObservableCollection<Food> AllFoods
        {
            get => _allFoods;
            set => SetProperty(ref _allFoods, value);
        }

        public ObservableCollection<Food> FoodsInOrder
        {
            get => _foodsInOrder;
            set => SetProperty(ref _foodsInOrder, value);
        }

        public Food SelectedFoodToAdd
        {
            get => _selectedFoodToAdd;
            set => SetProperty(ref _selectedFoodToAdd, value);
        }

        public Food SelectedFoodToRemove
        {
            get => _selectedFoodToRemove;
            set => SetProperty(ref _selectedFoodToRemove, value);
        }

        public CreateOrderWindowViewModel(Window window, User currentUser, Shift currentShift)
        {
            _currentWindow = window;
            _currentUser = currentUser;
            Order = new Order
            {
                Date = DateTime.Now,
                Status = 1,
                Shiftid = currentShift.Id
            };
            LoadOrderStatuses();
            Tables = new ObservableCollection<Table>(Service.GetContext().Tables.Where(t => t.Waiterontables.Any(w => w.Idwaiter == _currentUser.Id)).ToList());
            AllFoods = new ObservableCollection<Food>(Service.GetContext().Foods.ToList());
            FoodsInOrder = new ObservableCollection<Food>();
            _isOrderSaved = false;
            CurrentShift = currentShift;
        }

        private void LoadOrderStatuses()
        {
            var allStatuses = Service.GetContext().Orderstatuses.ToList();
            var filteredStatuses = allStatuses.Where(s => s.Name != "Готов" && s.Name != "Готовится").ToList();
            OrderStatuses = new ObservableCollection<Orderstatus>(filteredStatuses);
        }

        public IRelayCommand SaveOrderCommand => new RelayCommand(SaveOrder);

        private void SaveOrder()
        {
            if (!_isOrderSaved)
            {
                Service.GetContext().Orders.Add(Order);
                Service.GetContext().SaveChanges();
                _isOrderSaved = true;
            }
            else
            {
                Service.GetContext().Orders.Update(Order);
            }

            foreach (var food in FoodsInOrder)
            {
                var foodOnOrder = new Foodonorder
                {
                    Idfood = food.Id,
                    Idorder = Order.Id
                };
                Service.GetContext().Foodonorders.Add(foodOnOrder);
            }

            Service.GetContext().SaveChanges();
            _currentWindow.Close();
        }

        public IRelayCommand AddFoodCommand => new RelayCommand(AddFood);

        private void AddFood()
        {
            if (SelectedFoodToAdd != null)
            {
                FoodsInOrder.Add(SelectedFoodToAdd);
                AllFoods.Remove(SelectedFoodToAdd);
                SelectedFoodToAdd = null;
            }
        }

        public IRelayCommand RemoveFoodCommand => new RelayCommand(RemoveFood);

        private void RemoveFood()
        {
            if (SelectedFoodToRemove != null)
            {
                AllFoods.Add(SelectedFoodToRemove);
                FoodsInOrder.Remove(SelectedFoodToRemove);
                SelectedFoodToRemove = null;
            }
        }
    }
}
