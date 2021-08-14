using HomeWork_17_WPF.ViewModel;
using System.Windows;

namespace HomeWork_17_WPF.View
{
    /// <summary>
    /// Логика взаимодействия для MoveMoneyWindow.xaml
    /// </summary>
    public partial class MoveMoneyWindow : Window
    {
        public MoveMoneyWindow()
        {
            InitializeComponent();
            gridView.DataContext = MoveMoneyViewModel.clientsTable.DefaultView;
        }
    }
}
