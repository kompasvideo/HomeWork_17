using HomeWork_17_WPF.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace HomeWork_17_WPF.ViewModel
{
    class MoveMoneyViewModel
    {
        /// <summary>
        /// Выбранный клинт в списке
        /// </summary>
        public Client SelectedClient { get; set; }
        /// <summary>
        /// Сумма перевода
        /// </summary>
        public uint MoneyMove { get; set; }
        /// <summary>
        /// Нажата кнопка "Ок"
        /// </summary>
        public ICommand bOK_Click
        {
            get
            {
                return new DelegateCommand((obj) =>
                {
                    if (SelectedClient != null)
                    {
                        Dictionary<Client, uint> client = new Dictionary<Client, uint>();
                        client.Add(SelectedClient, MoneyMove);
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
    }
}
