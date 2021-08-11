using System.Collections.Generic;
using DevExpress.Mvvm;
using System.Windows.Input;
using HomeWork_17_WPF.Model;

namespace HomeWork_17_WPF.ViewModel
{
    class RateViewModel : ViewModelBase
    {
        public static string[] MoneyRate { get; set; }

        public RateViewModel()
        {
        }
        /// <summary>
        /// Принимает параметр типа Client
        /// </summary>
        /// <param name="client"></param>
        public static void SetClient(Dictionary<Client, short> client)
        {
            foreach (KeyValuePair<Client, short> kvp in client)
            {
                Client l_client = kvp.Key;
                //MoneyRate = l_client.DepositClient.GetSumRate(l_client.Money);
                MoneyRate = l_client.GetSumRateExt();
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
                    foreach (System.Windows.Window window in System.Windows.Application.Current.Windows)
                    {
                        if (window.Title == "Расчёт %")
                        {
                            window.Close();
                        }
                    }
                });
            }
        }
    }
}
