using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using cafeapp1.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace cafeapp1.ViewModels
{
    public partial class EditOrderWindowViewModel : ViewModelBase
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

        public EditOrderWindowViewModel(Window window, User currentUser, Order order)
        {
            _currentWindow = window;
            _currentUser = currentUser;
            Order = order;
            LoadOrderStatuses();
            Tables = new ObservableCollection<Table>(Service.GetContext().Tables.Where(t => t.Waiterontables.Any(w => w.Idwaiter == _currentUser.Id)).ToList());
            AllFoods = new ObservableCollection<Food>(Service.GetContext().Foods.ToList());
            FoodsInOrder = new ObservableCollection<Food>(Service.GetContext().Foodonorders.Where(fo => fo.Idorder == Order.Id).Select(fo => fo.IdfoodNavigation).ToList());
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
            Service.GetContext().Orders.Update(Order);

            // Remove existing foodonorders for this order
            var existingFoodOnOrders = Service.GetContext().Foodonorders.Where(fo => fo.Idorder == Order.Id).ToList();
            Service.GetContext().Foodonorders.RemoveRange(existingFoodOnOrders);

            // Add new foodonorders
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