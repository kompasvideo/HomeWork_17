using HomeWork_17_WPF.Messages;
using HomeWork_17_WPF.Services;
using HomeWork_17_WPF.Model;
using HomeWork_17_WPF.ViewModel;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Data.Common;

namespace HomeWork_17_WPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            gridView.DataContext = MainViewModel.clientsTable.DefaultView;
        }


        /// <summary>
        /// Подписывается на сообщение ReturnAddClient
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Messenger.Default.Register<Client>(MainViewModel.ReturnAddClient);
            Messenger.Default.Register<Dictionary<DataRow, int>>(MainViewModel.ReturnMoveMoney);
            Messenger.Default.Register<Deposit>(MainViewModel.ReturnAddDepositNoCapitalize);
            Messenger.Default.Register<DepositPlusCapitalize>(MainViewModel.ReturnAddDepositCapitalize);
            Messenger.Default.Register<BankDepartment>(AddDepositCapitalizeViewModel.SetBankDepartment);
            Messenger.Default.Register<Dictionary<BankDepartment, uint>>(AddDepositNoCapitalizeViewModel.SetBankDepartment);
            Messenger.Default.Register<Dictionary<DataRow, short>>(RateViewModel.SetClient);
            Messenger.Default.Register<MessageParam>(Message.SendTo);
        }

        /// <summary>
        /// Отписывается от сообщение ReturnAddClient
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            Messenger.Default.Unregister<Client>(MainViewModel.ReturnAddClient);
            Messenger.Default.Unregister<Dictionary<DataRow, int>>(MainViewModel.ReturnMoveMoney);
            Messenger.Default.Unregister<Deposit>(MainViewModel.ReturnAddDepositNoCapitalize);
            Messenger.Default.Unregister<DepositPlusCapitalize>(MainViewModel.ReturnAddDepositCapitalize);
            Messenger.Default.Unregister<BankDepartment>(AddDepositCapitalizeViewModel.SetBankDepartment);
            Messenger.Default.Unregister<Dictionary<BankDepartment, uint>>(AddDepositNoCapitalizeViewModel.SetBankDepartment);
            Messenger.Default.Unregister<Dictionary<DataRow, short>>(RateViewModel.SetClient);
            Messenger.Default.Unregister<MessageParam>(Message.SendTo);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Messenger.Default.Send(new MessageParam(DateTime.Now, MessageType.Save, $"Закрытие"));
        }
    }
}
