using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using cafeapp1.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace cafeapp1.ViewModels
{
    public partial class AssignTablesWindowViewModel : ViewModelBase
    {
        private Window _currentWindow;
        private ObservableCollection<User> _waiters;
        private User _selectedWaiter;
        private ObservableCollection<Waiterontable> _waiterTables;
        private Waiterontable _selectedWaiterTable;
        private ObservableCollection<Table> _availableTables;
        private Table _selectedAvailableTable;

        public ObservableCollection<User> Waiters
        {
            get => _waiters;
            set => SetProperty(ref _waiters, value);
        }

        public User SelectedWaiter
        {
            get => _selectedWaiter;
            set
            {
                SetProperty(ref _selectedWaiter, value);
                LoadWaiterTables();
            }
        }

        public ObservableCollection<Waiterontable> WaiterTables
        {
            get => _waiterTables;
            set => SetProperty(ref _waiterTables, value);
        }

        public Waiterontable SelectedWaiterTable
        {
            get => _selectedWaiterTable;
            set => SetProperty(ref _selectedWaiterTable, value);
        }

        public ObservableCollection<Table> AvailableTables
        {
            get => _availableTables;
            set => SetProperty(ref _availableTables, value);
        }

        public Table SelectedAvailableTable
        {
            get => _selectedAvailableTable;
            set => SetProperty(ref _selectedAvailableTable, value);
        }

        public AssignTablesWindowViewModel(Window window)
        {
            _currentWindow = window;
            Waiters = new ObservableCollection<User>(Service.GetContext().Users.Where(u => u.Roleid == 2).ToList());
            AvailableTables = new ObservableCollection<Table>(Service.GetContext().Tables.ToList());
        }

        private void LoadWaiterTables()
        {
            if (SelectedWaiter != null)
            {
                var waiterTables = Service.GetContext().Waiterontables
                    .Where(wt => wt.Idwaiter == SelectedWaiter.Id)
                    .ToList();

                WaiterTables = new ObservableCollection<Waiterontable>(waiterTables);

                // Обновляем список доступных столов
                AvailableTables = new ObservableCollection<Table>(Service.GetContext().Tables.Where(t => !waiterTables.Select(wt => wt.IdtableNavigation).Contains(t)).ToList());
            }
        }

        public IRelayCommand RemoveTableCommand => new RelayCommand(RemoveTable);

        private void RemoveTable()
        {
            if (SelectedWaiterTable != null)
            {
                Service.GetContext().Waiterontables.Remove(SelectedWaiterTable);
                Service.GetContext().SaveChanges();
                LoadWaiterTables();
            }
        }

        public IRelayCommand AddTableCommand => new RelayCommand(AddTable);

        private void AddTable()
        {
            if (SelectedWaiter != null && SelectedAvailableTable != null)
            {
                var waiterTable = new Waiterontable
                {
                    Idwaiter = SelectedWaiter.Id,
                    Idtable = SelectedAvailableTable.Id
                };

                Service.GetContext().Waiterontables.Add(waiterTable);
                Service.GetContext().SaveChanges();
                LoadWaiterTables();
            }
        }
    }
}
