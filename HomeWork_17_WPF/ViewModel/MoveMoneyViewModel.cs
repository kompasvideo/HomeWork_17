using HomeWork_17_WPF.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace HomeWork_17_WPF.ViewModel
{
    class MoveMoneyViewModel
    {
        static bool isLoad = false;
        static SqlConnection con;
        static SqlDataAdapter adapter;
        public static DataTable clientsTable { get; set; }
        static DataSet ds;

        /// <summary>
        /// Выбранный клиент в списке
        /// </summary>
        static DataRow selectDataRow;
        public static int SelectedClientID { get; set; }

        /// <summary>
        /// Сумма перевода
        /// </summary>
        public int MoneyMove { get; set; }


        public MoveMoneyViewModel()
        {
            if (!isLoad)
            {
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
        /// Нажата кнопка "Ок"
        /// </summary>
        public ICommand bOK_Click
        {
            get
            {
                return new DelegateCommand((obj) =>
                {
                    if (SelectedClientID != 0)
                    {
                        Dictionary<DataRow, int> client = new Dictionary<DataRow, int>();
                        client.Add(selectDataRow, MoneyMove);
                        Messenger.Default.Send(client);
                        foreach (System.Windows.Window window in System.Windows.Application.Current.Windows)
                        {
                            if (window.Title == "Перевести между счетами")
                            {
                                window.Close();
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Не выбран счёт для перевода", "Перевести на другой счёт");
                    }
                    isLoad = false;
                });
            }
        }
        /// <summary>
        /// Нажата кнопка "Отмена"
        /// </summary>
        public ICommand bCancel_Click
        {
            get
            {
                return new DelegateCommand((obj) =>
                {
                    foreach (System.Windows.Window window in System.Windows.Application.Current.Windows)
                    {
                        if (window.Title == "Перевести между счетами")
                        {
                            window.Close();
                        }
                    }
                });
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
                    if (obj is DataRowView client)
                    {
                        selectDataRow = client.Row;
                        SelectedClientID = (int)client[0];
                    }
                });
                return a;
            }
        }
    }
}
