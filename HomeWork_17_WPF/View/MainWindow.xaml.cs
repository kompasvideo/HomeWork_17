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
        SqlConnection con;
        SqlDataAdapter adapter;
        DataTable clientsTable;
        public MainWindow()
        {
            InitializeComponent();
            Preparing();
        }
        private void Preparing()
        {
            #region Init
            string dataProvider =
                ConfigurationManager.AppSettings["provider"];
            string connectionString =
                ConfigurationManager.ConnectionStrings["BankSqlProvider"].ConnectionString;

            DbProviderFactory factory = DbProviderFactories.GetFactory(dataProvider);
            using (DbConnection connection = factory.CreateConnection())
            {
                if (connection == null)
                {
                    ShowError("Connection");
                    return;
                }
                connection.ConnectionString = connectionString;
                connection.Open();

                con = connection as SqlConnection;
                if (con == null)
                {
                    ShowError("Connection");
                    return;
                }
                adapter = new SqlDataAdapter("SELECT * FROM Clients", con);
                DataSet ds = new DataSet("bank");
                //adapter.Fill(ds);
                clientsTable = new DataTable("Table");
                ds.Tables.Add(clientsTable);
                adapter.Fill(ds.Tables["Table"]);

                gridView.DataContext = clientsTable.DefaultView;
            }
            #endregion
        }
        private static void ShowError(string objectName)
        {
            
        }

        /// <summary>
        /// Выполняется при загрузке ListView
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListView_Loaded(object sender, RoutedEventArgs e)
        {
            //MainViewModel.Source = (ListCollectionView)CollectionViewSource.GetDefaultView(LVClients.ItemsSource);
            //MainViewModel.Source.Filter = new Predicate<object>(MainViewModel.MyFilter);
        }

        /// <summary>
        /// Подписывается на сообщение ReturnAddClient
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Messenger.Default.Register<Client>(MainViewModel.ReturnAddClient);
            Messenger.Default.Register<Dictionary<Client, uint>>(MainViewModel.ReturnMoveMoney);
            Messenger.Default.Register<Deposit>(MainViewModel.ReturnAddDepositNoCapitalize);
            Messenger.Default.Register<DepositPlusCapitalize>(MainViewModel.ReturnAddDepositCapitalize);
            Messenger.Default.Register<BankDepartment>(AddDepositCapitalizeViewModel.SetBankDepartment);
            Messenger.Default.Register<Dictionary<BankDepartment, uint>>(AddDepositNoCapitalizeViewModel.SetBankDepartment);
            Messenger.Default.Register<Dictionary<Client, short>>(RateViewModel.SetClient);
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
            Messenger.Default.Unregister<Dictionary<Client, uint>>(MainViewModel.ReturnMoveMoney);
            Messenger.Default.Unregister<Deposit>(MainViewModel.ReturnAddDepositNoCapitalize);
            Messenger.Default.Unregister<DepositPlusCapitalize>(MainViewModel.ReturnAddDepositCapitalize);
            Messenger.Default.Unregister<BankDepartment>(AddDepositCapitalizeViewModel.SetBankDepartment);
            Messenger.Default.Unregister<Dictionary<BankDepartment, uint>>(AddDepositNoCapitalizeViewModel.SetBankDepartment);
            Messenger.Default.Unregister<Dictionary<Client, short>>(RateViewModel.SetClient);
            Messenger.Default.Unregister<MessageParam>(Message.SendTo);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Messenger.Default.Send(new MessageParam(DateTime.Now, MessageType.Save, $"Закрытие"));
        }
    }
}
