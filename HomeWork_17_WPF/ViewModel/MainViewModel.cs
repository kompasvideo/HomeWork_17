using HomeWork_17_WPF.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DevExpress.Mvvm;
using System.Windows.Input;
using System.Windows;
using HomeWork_17_WPF.Exceptions;
using HomeWork_17_WPF.Services;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Data.Common;

namespace HomeWork_17_WPF.ViewModel
{
    class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// Названия департаментов банка
        /// </summary>
        public ObservableCollection<string> departments { get; set; } = new ObservableCollection<string>()
            {Const.personalName, Const.businessName, Const.VIPName};
        /// <summary>
        /// Список клиентов банка
        /// </summary>
        public static ObservableCollection<Client> clients { get; set; } = new ObservableCollection<Client>();
        Random rnd = new Random();
        /// <summary>
        /// Выбранный клинт в списке
        /// </summary>
        public static Client SelectedClient { get; set; }
        /// <summary>
        /// выбранный департамент в списке
        /// </summary>
        public static string SelectedDep { get; set; }
        /// <summary>
        /// CollectionViewSource для департаментов
        /// </summary>
        public static System.ComponentModel.ICollectionView Source;
        static bool isLoad = false;

        /// <summary>
        /// Имя выбранного клиента
        /// </summary>
        public string SelectClientName { get; set; }
        /// <summary>
        /// Сумма на счёте выбранного клиента
        /// </summary>
        public uint SelectClientMoney { get; set; }
        /// <summary>
        /// Тип клиента
        /// </summary>
        public string SelectClientType { get; set; }
        /// <summary>
        /// Вклад клиента
        /// </summary>
        public string SelectClientDeposit { get; set; }
        /// <summary>
        /// Ставка по вкладу
        /// </summary>
        public float SelectClientInterestRate { get; set; }
        /// <summary>
        /// Дата открытия
        /// </summary>
        public string SelectClientDataBegin { get; set; }
        /// <summary>
        /// На срок в днях
        /// </summary>
        public uint SelectClientDays { get; set; }

        public MainViewModel()
        {
            if (!isLoad)
            {
                //AddClientsToBank(3, 3, 3);
                isLoad = true;
            }
        }        

        /// <summary>
        /// Создаёт клиентов при запуске автоматически
        /// </summary>
        /// <param name="personal">Кол-во физ.лиц</param>
        /// <param name="business">Кол-во юр.лиц</param>
        /// <param name="vip">Кол-во VIP клиентов</param>
        void AddClientsToBank(int personal, int business, int vip)
        {
            // 
            for (int i = 0; i < personal; i++)
            {
                CreateClientsCollection<PersonalClient>();
            }

            // 
            for (int i = 0; i < business; i++)
            {
                CreateClientsCollection<BusinessClient>();
            }

            // 
            for (int i = 0; i < vip; i++)
            {
                CreateClientsCollection<VIPClient>();
            }
        }

        /// <summary>
        /// Добавляет клиентов в список при запуске автоматически
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void CreateClientsCollection<T>() where T : Client, new()
        {
            clients.Add(new T());
        }

        /// <summary>
        /// Команда "Выбор департамента в ListBox"
        /// </summary>
        public ICommand SelectedItemChangedCommand
        {
            get
            {
                var a = new DelegateCommand((obj) =>
                {
                    try
                    {
                        int id;
                        //if (Source != null)
                        //    Source.Filter = new Predicate<object>(MyFilter);
                        SqlConnection con;
                        //SqlDataAdapter adapter;
                        //DataTable clientsTable;
                        string dataProvider = ConfigurationManager.AppSettings["provider"];
                        string connectionString = ConfigurationManager.ConnectionStrings["BankSqlProvider"].ConnectionString;

                        DbProviderFactory factory = DbProviderFactories.GetFactory(dataProvider);
                        using (DbConnection connection = factory.CreateConnection())
                        {
                            if (connection == null)
                            {
                                MessageBox.Show("Ошибка Connection");
                                return;
                            }
                            connection.ConnectionString = connectionString;
                            //connection.Open();

                            con = connection as SqlConnection;
                            if (con == null)
                            {
                                MessageBox.Show("Ошибка Connection 2");
                                return;
                            }
                            string objStr = obj as string;
                            string strSql = $"SELECT * FROM Departments WHERE Name=N'{objStr}'";
                            //string strSql = $"SELECT * FROM Departments WHERE Name=N'Юр. лицо'";
                            SqlCommand sqlCommand = new SqlCommand();
                            sqlCommand.Connection = con;
                            sqlCommand.CommandText = CommandType.Text.ToString();
                            sqlCommand.CommandText = strSql;
                            sqlCommand.Connection.Open();
                            id = (int)sqlCommand.ExecuteScalar();
                        }
                    }
                    catch (Exception) 
                    {
                        MessageBox.Show("Исключение");
                    }
                });
                return a;
            }
        }

        /// <summary>
        /// Команда "Выбор клинта в ListView"
        /// </summary>
        public ICommand LVSelectedItemChangedCommand
        {
            get
            {
                var a = new DelegateCommand((obj) =>
                {
                    Client client = obj as Client;
                    if (client != null)
                    {
                        SelectClientName = client.Name;
                        SelectClientMoney = client.Money;
                        SelectClientType = client.Status;
                        SelectClientDeposit = client.DepositClientStr;
                        if (client.DepositClient != null)
                        {
                            SelectClientInterestRate = client.DepositClient.InterestRate;
                            SelectClientDataBegin = client.DepositClient.DateBegin.ToLongDateString();
                            SelectClientDays = client.DepositClient.Days;
                        }
                        else
                        {
                            SelectClientInterestRate = 0;
                            SelectClientDataBegin = "";
                            SelectClientDays = 0;
                        }
                    }
                });
                return a;
            }
        }

        /// <summary>
        /// Фильтр для показа клинтов по департаментам
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool MyFilter(object obj)
        {
            var c = obj as Client;
            if (SelectedDep != null)
            {
                switch (SelectedDep)
                {
                    case Const.personalName:
                        if (c.BankDepartmentProp == BankDepartment.PersonalDepartment)
                            return true;
                        else
                            return false;
                    case Const.businessName:
                        if (c.BankDepartmentProp == BankDepartment.BusinessDepartment)
                            return true;
                        else
                            return false;
                    case Const.VIPName:
                        if (c.BankDepartmentProp == BankDepartment.VIPDepartment)
                            return true;
                        else
                            return false;
                }
            }
            return true;
        }


        #region Открыть счёт
        /// <summary>
        /// Открыть счёт
        /// </summary>
        public ICommand AddAccount_Click
        {
            get
            {
                var a = new DelegateCommand((obj) =>
                {
                    var displayRootRegistry = (Application.Current as App).displayRootRegistry;

                    var addAccountViewModel = new AddAccountViewModel();
                    displayRootRegistry.ShowModalPresentation(addAccountViewModel);
                });
                return a;
            }
        }
        /// <summary>
        /// Возвращяет Client из диалогового окна AddClient
        /// </summary>
        /// <param name="employee"></param>
        public static void ReturnAddClient(Client client)
        {
            clients.Add(client);
            Messenger.Default.Send(new MessageParam(DateTime.Now, MessageType.AddAccount, $"Открыт счёт для '{client.Name}' на сумму '{client.Money}'"));
        }
        #endregion


        #region Закрыть счёт
        /// <summary>
        /// Закрыть счёт
        /// </summary>
        public ICommand CloseAccount_Click
        {
            get
            {
                var a = new DelegateCommand((obj) =>
                {
                    try
                    {
                        if (SelectedClient == null)
                        {
                            throw new NoSelectClientException("Не выбран клиент");
                        }
                        else
                        {
                            if (MessageBox.Show($"Закрыть счёт для   '{SelectedClient.Name}'", "Закрыть счёт", MessageBoxButton.YesNo) == MessageBoxResult.No)
                                return;
                            clients.Remove(SelectedClient);
                            Messenger.Default.Send(new MessageParam(DateTime.Now, MessageType.CloseAccount, $"Закрыт счёт для '{SelectedClient.Name}' на сумму '{SelectedClient.Money}'"));
                        }
                    }
                    catch (NoSelectClientException ex)
                    {
                        MessageBox.Show(ex.Message, "Закрыть счёт");
                    }
                });
                return a;
            }
        }
        #endregion


        #region Перевести на другой счёт
        /// <summary>
        /// Перевести на другой счёт
        /// </summary>
        public ICommand MoveMoney_Click
        {
            get
            {
                var a = new DelegateCommand((obj) =>
                {
                    try
                    {
                        if (SelectedClient == null)
                        {
                            throw new NoSelectClientException("Не выбран клиент для перевода");
                        }
                        else
                        {
                            var displayRootRegistry = (Application.Current as App).displayRootRegistry;

                            var moveMoneyViewModel = new MoveMoneyViewModel();
                            displayRootRegistry.ShowModalPresentation(moveMoneyViewModel);
                        }
                        //else
                        //    MessageBox.Show("Не выбран счёт для перевода", "Перевести на другой счёт");
                    }
                    catch (NoSelectClientException ex)
                    {
                        MessageBox.Show(ex.Message, "Перевести на другой счёт");
                    }
                });
                return a;
            }
        }
        /// <summary>
        /// Возвращяет Client из диалогового окна MoveMoney
        /// </summary>
        /// <param name="employee"></param>
        public static void ReturnMoveMoney(Dictionary<Client, uint> client)
        {
            uint moveMoney;
            Client moveClient;
            foreach (KeyValuePair<Client, uint> kvp in client)
            {
                moveClient = kvp.Key;
                moveMoney = kvp.Value;
                if (SelectedClient.Money >= moveMoney)
                {
                    Client clientMinus = SelectedClient - moveMoney;
                    Client clientPlus = moveClient + moveMoney;
                    SelectedClient.Money = clientMinus.Money;
                    moveClient.Money = clientPlus.Money;
                    Source.Filter = new Predicate<object>(MyFilter);
                    Messenger.Default.Send(new MessageParam(DateTime.Now, MessageType.MoveMoney, $"Переведена сумма '{moveClient.Money}' с счёта '{SelectedClient.Name}' на счёт '{moveClient.Name}'"));
                }
                else
                {
                    MessageBox.Show($"На счёту клиента {SelectedClient} недостаточно средств", "Перевести на другой счёт");
                }
            }
            //SelectedClient.Money = 
        }
        #endregion


        #region Открыть вклад без капитализации %
        /// <summary>
        /// Открыть вклад без капитализации %
        /// </summary>
        public ICommand AddDepositNoCapitalize_Click
        {
            get
            {
                var a = new DelegateCommand((obj) =>
                {
                    try
                    { 
                        if (SelectedClient == null)
                        {
                            throw new NoSelectClientException("Не выбран клиент");
                        }
                        else
                        { 
                            var displayRootRegistry = (Application.Current as App).displayRootRegistry;
                            var addDepositNoCapitalizeViewModel = new AddDepositNoCapitalizeViewModel();
                            Dictionary<BankDepartment, uint> bd = new Dictionary<BankDepartment, uint>();
                            switch (SelectedClient.BankDepartmentProp)
                        {
                            case BankDepartment.BusinessDepartment:
                                bd.Add(BankDepartment.BusinessDepartment, 0);
                                Messenger.Default.Send(bd);
                                displayRootRegistry.ShowModalPresentation(addDepositNoCapitalizeViewModel);
                                break;
                            case BankDepartment.PersonalDepartment:
                                bd.Add(BankDepartment.PersonalDepartment, 0);
                                Messenger.Default.Send(bd);
                                displayRootRegistry.ShowModalPresentation(addDepositNoCapitalizeViewModel);
                                break;
                            case BankDepartment.VIPDepartment:
                                bd.Add(BankDepartment.VIPDepartment, 0);
                                Messenger.Default.Send(bd);
                                displayRootRegistry.ShowModalPresentation(addDepositNoCapitalizeViewModel);
                                break;
                        }
                            if (SelectedClient.DepositClient != null)
                        {
                            SelectClientDeposit = SelectedClient.DepositClientStr;
                            SelectClientInterestRate = SelectedClient.DepositClient.InterestRate;
                            SelectClientDataBegin = SelectedClient.DepositClient.DateBegin.ToLongDateString();
                            SelectClientDays = SelectedClient.DepositClient.Days;
                        }
                        }
                        //else
                        //    MessageBox.Show("Не выбран клиент", "Открыть вклад без капитализации %");
                    }
                    catch (NoSelectClientException ex)
                    {
                        MessageBox.Show(ex.Message, "Перевести на другой счёт");
                    }
                });
                return a;
            }
        }

        /// <summary>
        /// Возвращяет Deposit из окна AddDepositNoCapitalizeWindow
        /// </summary>
        /// <param name="deposit"></param>
        public static void ReturnAddDepositNoCapitalize(Deposit deposit)
        {
            SelectedClient.DepositClient = deposit;
            SelectedClient.DepositClientStr = "вклад без капитализации %";
            Messenger.Default.Send(new MessageParam(DateTime.Now, MessageType.AddDepositNoCapitalize, $"Открыт вклад без капитализации % для '{SelectedClient.Name}'"));
        }
        #endregion


        #region Открыть вклад с капитализацией %
        /// <summary>
        /// Открыть вклад с капитализацией %
        /// </summary>
        public ICommand AddDepositCapitalize_Click
        {
            get
            {
                var a = new DelegateCommand((obj) =>
                {
                    try
                    { 
                        if (SelectedClient == null)
                        {
                            throw new NoSelectClientException("Не выбран клиент");
                        }
                        else
                        { 
                            var displayRootRegistry = (Application.Current as App).displayRootRegistry;
                            var addDepositCapitalizeViewModel = new AddDepositCapitalizeViewModel();
                            switch (SelectedClient.BankDepartmentProp)
                        {
                            case BankDepartment.BusinessDepartment:
                                Messenger.Default.Send(BankDepartment.BusinessDepartment);
                                displayRootRegistry.ShowModalPresentation(addDepositCapitalizeViewModel);
                                break;
                            case BankDepartment.PersonalDepartment:
                                Messenger.Default.Send(BankDepartment.PersonalDepartment);
                                displayRootRegistry.ShowModalPresentation(addDepositCapitalizeViewModel);
                                break;
                            case BankDepartment.VIPDepartment:
                                Messenger.Default.Send(BankDepartment.VIPDepartment);
                                displayRootRegistry.ShowModalPresentation(addDepositCapitalizeViewModel);
                                break;
                        }
                            if (SelectedClient.DepositClient != null)
                        {
                            SelectClientDeposit = SelectedClient.DepositClientStr;
                            SelectClientInterestRate = SelectedClient.DepositClient.InterestRate;
                            SelectClientDataBegin = SelectedClient.DepositClient.DateBegin.ToLongDateString();
                            SelectClientDays = SelectedClient.DepositClient.Days;
                        }
                        }
                        //else
                        //    MessageBox.Show("Не выбран клиент", "Открыть вклад с капитализацией %");
                    }
                    catch (NoSelectClientException ex)
                    {
                        MessageBox.Show(ex.Message, "Перевести на другой счёт");
                    }
                });
                return a;
            }
        }

        /// <summary>
        /// Возвращяет Deposit из окна AddDepositNoCapitalizeWindow
        /// </summary>
        /// <param name="deposit"></param>
        public static void ReturnAddDepositCapitalize(Deposit deposit)
        {
            SelectedClient.DepositClient = deposit;
            SelectedClient.DepositClientStr = "вклад с капитализацией %";
            //Messenger.Default.Send($"{DateTime.Now} Открыт вклад c капитализацией % для '{SelectedClient.Name}'");
            Messenger.Default.Send(new MessageParam(DateTime.Now, MessageType.AddDepositCapitalize, $"Открыт вклад c капитализацией % для '{SelectedClient.Name}'"));
        }
        #endregion


        #region Показать окно с расчётом %
        /// <summary>
        /// Показать окно с расчётом %
        /// </summary>
        public ICommand RateView_Click
        {
            get
            {
                var a = new DelegateCommand((obj) =>
                {
                    try 
                    { 
                        if (SelectedClient == null)
                        {
                            throw new NoSelectClientException("Не выбран клиент");
                        }
                        else
                        { 
                            if (SelectedClient.DepositClient != null)
                            {
                                var displayRootRegistry = (Application.Current as App).displayRootRegistry;

                                var rateViewModel = new RateViewModel();
                                Dictionary<Client, short> client = new Dictionary<Client, short>();
                                client.Add(SelectedClient, 0);
                                Messenger.Default.Send(client);
                                displayRootRegistry.ShowModalPresentation(rateViewModel);
                                Messenger.Default.Send(new MessageParam(DateTime.Now, MessageType.RateView, $"Показано окно с расчётом % для '{SelectedClient.Name}'"));
                            }
                        }
                    }
                    catch (NoSelectClientException ex)
                    {
                        MessageBox.Show(ex.Message, "Перевести на другой счёт");
                    }
                });
                return a;
            }
        }
        #endregion











        #region Создать Log
        /// <summary>
        /// Создать Log
        /// </summary>
        public ICommand CreateLog_Click
        {
            get
            {
                return new DelegateCommand((obj) => SaveMessages.Save());               
            }
        }
        #endregion

        #region Загрузить Log
        /// <summary>
        /// Загрузить Log
        /// </summary>
        public ICommand LoadLog_Click
        {
            get
            {
                return new DelegateCommand(async (obj) => await SaveMessages.Load());                 
            }
        }
        #endregion

    }
}
