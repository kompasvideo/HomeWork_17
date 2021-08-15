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
        /// Выбранный клинт в списке
        /// </summary>
        public static Client SelectedClient { get; set; }
        static bool isLoad = false;

        /// <summary>
        /// Имя выбранного клиента
        /// </summary>
        static public string SelectedClientName { get; set; }
        public string SelectClientName { get; set; }
        /// <summary>
        /// Сумма на счёте выбранного клиента
        /// </summary>
        public int SelectClientMoney { get; set; }
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
        public double SelectClientInterestRate { get; set; }
        /// <summary>
        /// Дата открытия
        /// </summary>
        public string SelectClientDataBegin { get; set; }
        /// <summary>
        /// На срок в днях
        /// </summary>
        public int SelectClientDays { get; set; }

        static SqlConnection con;
        static SqlDataAdapter adapter;
        public static DataTable clientsTable { get; set; }
        static DataSet ds;
        static DataRow selectDataRow;
        static Dictionary<string, int> departmentsDB;
        public static int SelectedClientID { get; set; }

        public MainViewModel()
        {
            if (!isLoad)
            {
                //AddClientsToBank(3, 3, 3);
                LoadInDataBase();
                isLoad = true;
            }
        }

        /// <summary>
        /// Первоначальная загрузка клиентов из БД
        /// </summary>
        private void LoadInDataBase()
        {
            try
            {
                string dataProvider =
                    ConfigurationManager.AppSettings["provider"];
                string connectionString =
                    ConfigurationManager.ConnectionStrings["BankSqlProvider"].ConnectionString;

                DbProviderFactory factory = DbProviderFactories.GetFactory(dataProvider);
                using (DbConnection connection = factory.CreateConnection())
                {
                    connection.ConnectionString = connectionString;
                    connection.Open();

                    con = connection as SqlConnection;
                    adapter = new SqlDataAdapter("SELECT * FROM Clients", con);
                    ds = new DataSet("bank");
                    clientsTable = new DataTable("Table");
                    ds.Tables.Add(clientsTable);
                    adapter.Fill(ds.Tables["Table"]);

                    // Получаем список департаментов из БД
                    departmentsDB = new Dictionary<string, int>();
                    string sql = "SELECT * FROM Departments";
                    SqlCommand sqlCommand = new SqlCommand(sql, con);
                    using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                    {
                        while (sqlDataReader.Read())
                        {
                            departmentsDB.Add((string)sqlDataReader["Name"], (int)sqlDataReader["Id"]);
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                string errorMessage = "";
                foreach (SqlError sqlError in ex.Errors)
                {
                    errorMessage += sqlError.Message + " (error: " + sqlError.Number.ToString() + ")" + Environment.NewLine;
                    if (sqlError.Number == 18452)
                    {
                        MessageBox.Show("Invalid Login Detected");
                    }
                }
                MessageBox.Show(errorMessage);
            }
            finally
            {
                con.Close();
            }
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
                        string dataProvider = ConfigurationManager.AppSettings["provider"];
                        string connectionString = ConfigurationManager.ConnectionStrings["BankSqlProvider"].ConnectionString;

                        DbProviderFactory factory = DbProviderFactories.GetFactory(dataProvider);
                        using (DbConnection connection = factory.CreateConnection())
                        {
                            connection.ConnectionString = connectionString;
                            con = connection as SqlConnection;
                            string objStr = obj as string;
                            string strSql = $"SELECT * FROM Departments WHERE Name=N'{objStr}'";
                            SqlCommand sqlCommand = new SqlCommand();
                            sqlCommand.Connection = con;
                            sqlCommand.CommandType = CommandType.Text;
                            sqlCommand.CommandText = strSql;
                            sqlCommand.Connection.Open();
                            id = (int)sqlCommand.ExecuteScalar();

                            string sqlStr = $"SELECT * FROM Clients WHERE Department={id}";
                            clientsTable.Clear();
                            sqlCommand = new SqlCommand();
                            sqlCommand.Connection = con;
                            sqlCommand.CommandType = CommandType.Text;
                            sqlCommand.CommandText = sqlStr;
                            adapter.SelectCommand = sqlCommand;
                            adapter.Fill(ds.Tables["Table"]);

                        }
                    }
                    catch (SqlException ex)
                    {
                        string errorMessage = "";
                        foreach (SqlError sqlError in ex.Errors)
                        {
                            errorMessage += sqlError.Message + " (error: " + sqlError.Number.ToString() + ")" + Environment.NewLine;
                            if (sqlError.Number == 18452)
                            {
                                MessageBox.Show("Invalid Login Detected");
                            }
                        }
                        MessageBox.Show(errorMessage);
                    }
                    finally
                    {
                        con.Close();
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
                int z = new Int32();
                var a = new DelegateCommand((obj) =>
                {
                    try
                    {
                        if (obj is DataRowView client)
                        {
                            selectDataRow = client.Row;
                            SelectedClientID = (int)client[0];
                            SelectClientName = SelectedClientName = (string)client[1];
                            SelectClientMoney = (int)client[2];
                            switch ((int)client[3])
                            {
                                case 1:
                                    SelectClientType = "Физ. лицо";
                                    break;
                                case 2:
                                    SelectClientType = "Юр. лицо";
                                    break;
                                case 3:
                                    SelectClientType = "VIP";
                                    break;
                            }
                            if ((int)client[4] > 0)
                            {
                                switch ((int)client[4])
                                {
                                    case 1:
                                        SelectClientDeposit = "вклад без капитализации %";
                                        break;
                                    case 2:
                                        SelectClientDeposit = "вклад с капитализацией %";
                                        break;
                                }
                                SelectClientInterestRate = (double)client[7];
                                SelectClientDataBegin = ((DateTime)client[5]).ToShortDateString();
                                SelectClientDays = (int)client[6];
                            }
                            else
                            {
                                SelectClientDeposit = " ";
                                SelectClientInterestRate = 0;
                                SelectClientDataBegin = " ";
                                SelectClientDays = 0;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                });
                return a;
            }
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
            try
            {
                int maxID = 0;
                foreach (DataRow rows in clientsTable.Rows)
                {
                    int id = (int)rows[0];
                    if (maxID < id)
                    {
                        maxID = id;
                    }
                }
                int ID_department = 0;
                departmentsDB.TryGetValue(client.Status, out ID_department);
                DataRow clientRow = clientsTable.NewRow();
                Object[] rowRecord = new Object[5];
                rowRecord[0] = maxID + 1;
                rowRecord[1] = client.Name;
                rowRecord[2] = client.Money;
                rowRecord[3] = ID_department;
                rowRecord[4] = "0";
                clientRow.ItemArray = rowRecord;
                clientsTable.Rows.Add(clientRow);

                //Вставляет в таблицу Clients новую строку
                try
                {
                    string dataProvider = ConfigurationManager.AppSettings["provider"];
                    string connectionString = ConfigurationManager.ConnectionStrings["BankSqlProvider"].ConnectionString;

                    DbProviderFactory factory = DbProviderFactories.GetFactory(dataProvider);
                    using (DbConnection connection = factory.CreateConnection())
                    {
                        connection.ConnectionString = connectionString;
                        con = connection as SqlConnection;
                        string strSql = $"Insert Into Clients (Name, Money, Department, Deposit) Values ('{rowRecord[1]}','{rowRecord[2]}','{rowRecord[3]}','{rowRecord[4]}')";
                        SqlCommand sqlCommand = new SqlCommand();
                        sqlCommand.Connection = con;
                        sqlCommand.CommandType = CommandType.Text;
                        sqlCommand.CommandText = strSql;
                        sqlCommand.Connection.Open();
                        sqlCommand.ExecuteNonQuery();
                    }
                }
                catch (SqlException ex)
                {
                    string errorMessage = "";
                    foreach (SqlError sqlError in ex.Errors)
                    {
                        errorMessage += sqlError.Message + " (error: " + sqlError.Number.ToString() + ")" + Environment.NewLine;
                        if (sqlError.Number == 18452)
                        {
                            MessageBox.Show("Invalid Login Detected");
                        }
                    }
                    MessageBox.Show(errorMessage);
                }
                finally
                {
                    con.Close();
                }

                Messenger.Default.Send(new MessageParam(DateTime.Now, MessageType.AddAccount, $"Открыт счёт для '{client.Name}' на сумму '{client.Money}'"));
            }
            catch (SqlException ex)
            {
                string errorMessage = "";
                foreach (SqlError sqlError in ex.Errors)
                {
                    errorMessage += sqlError.Message + " (error: " + sqlError.Number.ToString() + ")" + Environment.NewLine;
                    if (sqlError.Number == 18452)
                    {
                        MessageBox.Show("Invalid Login Detected");
                    }
                }
                MessageBox.Show(errorMessage);
            }
            catch (Exception e)
            {
                MessageBox.Show("Исключение " + e.Message);
            }
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
                        if (SelectedClientID == 0)
                        {
                            throw new NoSelectClientException("Не выбран клиент");
                        }
                        else
                        {
                            if (MessageBox.Show($"Закрыть счёт для   '{SelectClientName}'", "Закрыть счёт", MessageBoxButton.YesNo) == MessageBoxResult.No)
                                return;
                            //clients.Remove(SelectedClient);
                            //удалить клиента из таблицы
                            clientsTable.Rows.Remove(selectDataRow);

                            //Удаляет из таблицы Clients строку
                            try
                            {
                                string dataProvider = ConfigurationManager.AppSettings["provider"];
                                string connectionString = ConfigurationManager.ConnectionStrings["BankSqlProvider"].ConnectionString;

                                DbProviderFactory factory = DbProviderFactories.GetFactory(dataProvider);
                                using (DbConnection connection = factory.CreateConnection())
                                {
                                    connection.ConnectionString = connectionString;
                                    con = connection as SqlConnection;
                                    string strSql = $"DELETE FROM Clients WHERE Id='{SelectedClientID}'";
                                    SqlCommand sqlCommand = new SqlCommand();
                                    sqlCommand.Connection = con;
                                    sqlCommand.CommandType = CommandType.Text;
                                    sqlCommand.CommandText = strSql;
                                    sqlCommand.Connection.Open();
                                    sqlCommand.ExecuteNonQuery();
                                }
                            }
                            catch (SqlException ex)
                            {
                                string errorMessage = "";
                                foreach (SqlError sqlError in ex.Errors)
                                {
                                    errorMessage += sqlError.Message + " (error: " + sqlError.Number.ToString() + ")" + Environment.NewLine;
                                    if (sqlError.Number == 18452)
                                    {
                                        MessageBox.Show("Invalid Login Detected");
                                    }
                                }
                                MessageBox.Show(errorMessage);
                            }
                            finally
                            {
                                con.Close();
                            }
                            Messenger.Default.Send(new MessageParam(DateTime.Now, MessageType.CloseAccount, $"Закрыт счёт для '{SelectClientName}' на сумму '{SelectClientMoney}'"));
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
                        if (SelectedClientID == 0)
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
        public static void ReturnMoveMoney(Dictionary<DataRow, int> client)
        {
            int moveMoney;
            DataRow moveClient;
            foreach (KeyValuePair<DataRow, int> kvp in client)
            {
                moveClient = kvp.Key;
                moveMoney = kvp.Value;
                int id = (int)moveClient[0];
                foreach (DataRow rows in clientsTable.Rows)
                {
                    int l_id = (int)rows[0];
                    if (id == l_id)
                    {
                        moveClient = rows;
                    }
                }
                if ((int)selectDataRow[2] >= moveMoney)
                {
                    int client_1_NewMoney = (int)selectDataRow[2] - moveMoney;
                    int client_2_NewMoney = (int)moveClient[2] + moveMoney;
                    int client_1_Id = (int)selectDataRow[0];
                    int client_2_Id = (int)moveClient[0];
                    //Обновляет в таблице Clients поле Money
                    try
                    {
                        string dataProvider = ConfigurationManager.AppSettings["provider"];
                        string connectionString = ConfigurationManager.ConnectionStrings["BankSqlProvider"].ConnectionString;

                        DbProviderFactory factory = DbProviderFactories.GetFactory(dataProvider);
                        using (DbConnection connection = factory.CreateConnection())
                        {
                            connection.ConnectionString = connectionString;
                            con = connection as SqlConnection;

                            SqlTransaction transaction;

                            SqlCommand sqlCommand1 = new SqlCommand();
                            sqlCommand1.Connection = con;
                            sqlCommand1.CommandType = CommandType.Text;
                            sqlCommand1.CommandText = $"UPDATE Clients SET Money='{client_1_NewMoney}' WHERE Id='{client_1_Id}'";
                            //sqlCommand1.Connection.Open();

                            SqlCommand sqlCommand2 = new SqlCommand();
                            sqlCommand2.Connection = con;
                            sqlCommand2.CommandType = CommandType.Text;
                            sqlCommand2.CommandText = $"UPDATE Clients SET Money='{client_2_NewMoney}' WHERE Id='{client_2_Id}'";
                            //sqlCommand2.Connection.Open();
                            con.Open();
                            transaction = con.BeginTransaction(IsolationLevel.Serializable);
                            sqlCommand1.Transaction = transaction;
                            sqlCommand2.Transaction = transaction;
                            sqlCommand1.ExecuteNonQuery();
                            sqlCommand2.ExecuteNonQuery();
                            transaction.Commit();
                            selectDataRow[2] = client_1_NewMoney;
                            moveClient[2] = client_2_NewMoney;
                        }
                    }
                    catch (SqlException ex)
                    {
                        string errorMessage = "";
                        foreach (SqlError sqlError in ex.Errors)
                        {
                            errorMessage += sqlError.Message + " (error: " + sqlError.Number.ToString() + ")" + Environment.NewLine;
                            if (sqlError.Number == 18452)
                            {
                                MessageBox.Show("Invalid Login Detected");
                            }
                        }
                        MessageBox.Show(errorMessage);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    finally
                    {
                        con.Close();
                    }
                }
                else
                {
                    MessageBox.Show($"На счёту клиента '{SelectedClientName} недостаточно средств", "Перевести на другой счёт");
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
                        if (SelectedClientID == 0)
                        {
                            throw new NoSelectClientException("Не выбран клиент");
                        }
                        else
                        {
                            var displayRootRegistry = (Application.Current as App).displayRootRegistry;
                            var addDepositNoCapitalizeViewModel = new AddDepositNoCapitalizeViewModel();
                            Dictionary<BankDepartment, uint> bd = new Dictionary<BankDepartment, uint>();
                            int dep = (int)selectDataRow[3];
                            switch (dep)
                            {
                                case 2:
                                    bd.Add(BankDepartment.BusinessDepartment, 0);
                                    Messenger.Default.Send(bd);
                                    displayRootRegistry.ShowModalPresentation(addDepositNoCapitalizeViewModel);
                                    break;
                                case 1:
                                    bd.Add(BankDepartment.PersonalDepartment, 0);
                                    Messenger.Default.Send(bd);
                                    displayRootRegistry.ShowModalPresentation(addDepositNoCapitalizeViewModel);
                                    break;
                                case 3:
                                    bd.Add(BankDepartment.VIPDepartment, 0);
                                    Messenger.Default.Send(bd);
                                    displayRootRegistry.ShowModalPresentation(addDepositNoCapitalizeViewModel);
                                    break;
                            }
                        }
                    }
                    catch (NoSelectClientException ex)
                    {
                        MessageBox.Show(ex.Message, "Открыть вклад без капитализации %");
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
            DateTime dateTime = deposit.DateBegin;
            string date = $"{dateTime.Year}-{dateTime.Month}-{dateTime.Day}";
            int clientId = SelectedClientID;
            //Обновляет в таблице Clients поля Deposit, DateOpen, Days, Rate
            try
            {
                string dataProvider = ConfigurationManager.AppSettings["provider"];
                string connectionString = ConfigurationManager.ConnectionStrings["BankSqlProvider"].ConnectionString;

                DbProviderFactory factory = DbProviderFactories.GetFactory(dataProvider);
                using (DbConnection connection = factory.CreateConnection())
                {
                    connection.ConnectionString = connectionString;
                    con = connection as SqlConnection;
                    SqlCommand sqlCommand = new SqlCommand();
                    sqlCommand.Connection = con;
                    sqlCommand.CommandType = CommandType.Text;
                    sqlCommand.CommandText = 
                        $"UPDATE Clients SET Deposit='1', DateOpen ='{date}', Days='{(int)deposit.Days}', Rate='{(double)deposit.InterestRate}' WHERE Id='{clientId}'";
                    sqlCommand.Connection.Open();
                    sqlCommand.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                string errorMessage = "";
                foreach (SqlError sqlError in ex.Errors)
                {
                    errorMessage += sqlError.Message + " (error: " + sqlError.Number.ToString() + ")" + Environment.NewLine;
                    if (sqlError.Number == 18452)
                    {
                        MessageBox.Show("Invalid Login Detected");
                    }
                }
                MessageBox.Show(errorMessage);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                con.Close();
            }
            Messenger.Default.Send(new MessageParam(DateTime.Now, MessageType.AddDepositNoCapitalize, $"Открыт вклад без капитализации % для '{SelectedClientName}'"));
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
                        if (SelectedClientID == 0)
                        {
                            throw new NoSelectClientException("Не выбран клиент");
                        }
                        else
                        {
                            var displayRootRegistry = (Application.Current as App).displayRootRegistry;
                            var addDepositCapitalizeViewModel = new AddDepositCapitalizeViewModel();
                            int dep = (int)selectDataRow[3];
                            switch (dep)
                            {
                                case 2:
                                    Messenger.Default.Send(BankDepartment.BusinessDepartment);
                                    displayRootRegistry.ShowModalPresentation(addDepositCapitalizeViewModel);
                                    break;
                                case 1:
                                    Messenger.Default.Send(BankDepartment.PersonalDepartment);
                                    displayRootRegistry.ShowModalPresentation(addDepositCapitalizeViewModel);
                                    break;
                                case 3:
                                    Messenger.Default.Send(BankDepartment.VIPDepartment);
                                    displayRootRegistry.ShowModalPresentation(addDepositCapitalizeViewModel);
                                    break;
                            }                            
                        }
                    }
                    catch (NoSelectClientException ex)
                    {
                        MessageBox.Show(ex.Message, "Открыть вклад с капитализацией %");
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
            DateTime dateTime = deposit.DateBegin;
            string date = $"{dateTime.Year}-{dateTime.Month}-{dateTime.Day}";
            int clientId = SelectedClientID;
            //Обновляет в таблице Clients поля Deposit, DateOpen, Days, Rate
            try
            {
                string dataProvider = ConfigurationManager.AppSettings["provider"];
                string connectionString = ConfigurationManager.ConnectionStrings["BankSqlProvider"].ConnectionString;

                DbProviderFactory factory = DbProviderFactories.GetFactory(dataProvider);
                using (DbConnection connection = factory.CreateConnection())
                {
                    connection.ConnectionString = connectionString;
                    con = connection as SqlConnection;
                    SqlCommand sqlCommand = new SqlCommand();
                    sqlCommand.Connection = con;
                    sqlCommand.CommandType = CommandType.Text;
                    sqlCommand.CommandText =
                        $"UPDATE Clients SET Deposit='2', DateOpen ='{date}', Days='{(int)deposit.Days}', Rate='{(double)deposit.InterestRate}' WHERE Id='{clientId}'";
                    sqlCommand.Connection.Open();
                    sqlCommand.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                string errorMessage = "";
                foreach (SqlError sqlError in ex.Errors)
                {
                    errorMessage += sqlError.Message + " (error: " + sqlError.Number.ToString() + ")" + Environment.NewLine;
                    if (sqlError.Number == 18452)
                    {
                        MessageBox.Show("Invalid Login Detected");
                    }
                }
                MessageBox.Show(errorMessage);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                con.Close();
            }

            Messenger.Default.Send(new MessageParam(DateTime.Now, MessageType.AddDepositCapitalize, $"Открыт вклад c капитализацией % для '{SelectedClientName}'"));
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
                        if (SelectedClientID == 0)
                        {
                            throw new NoSelectClientException("Не выбран клиент");
                        }
                        else
                        {
                            if ((int)selectDataRow[4] > 0)
                            {
                                var displayRootRegistry = (Application.Current as App).displayRootRegistry;

                                var rateViewModel = new RateViewModel();
                                Dictionary<DataRow, short> client = new Dictionary<DataRow, short>();
                                client.Add(selectDataRow, 0);
                                Messenger.Default.Send(client);
                                displayRootRegistry.ShowModalPresentation(rateViewModel);
                                Messenger.Default.Send(new MessageParam(DateTime.Now, MessageType.RateView, $"Показано окно с расчётом % для '{SelectedClientName}'"));
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
