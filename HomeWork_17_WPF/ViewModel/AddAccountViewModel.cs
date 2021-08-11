using DevExpress.Mvvm;
using System.Windows.Input;
using HomeWork_17_WPF.Model;
using System.Windows;

namespace HomeWork_17_WPF.ViewModel
{
    class AddAccountViewModel : ViewModelBase
    {
        /// <summary>
        /// Выбранный департамент в списке
        /// </summary>
        public static string SelectedDep { get; set; }
        /// <summary>
        /// Имя клиента
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Сумма на счёте
        /// </summary>
        public uint Money { get; set; }
        public AddAccountViewModel()
        {
        }
        public AddAccountViewModel(string Name, uint Money)
        {
            this.Name = Name;
            this.Money = Money;
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
                    if (SelectedDep == null)
                    {
                        MessageBox.Show("Не выбран тип счёта", "Ошибка");
                    }
                    else
                    {
                        Client client;
                        switch (SelectedDep)
                        {
                            case Const.personalName:
                                client = new PersonalClient(Name, Money);
                                Messenger.Default.Send(client);
                                break;
                            case Const.businessName:
                                client = new BusinessClient(Name, Money);
                                Messenger.Default.Send(client);
                                break;
                            case Const.VIPName:
                                client = new VIPClient(Name, Money);
                                Messenger.Default.Send(client);
                                break;
                        }
                        foreach (System.Windows.Window window in System.Windows.Application.Current.Windows)
                        {
                            if (window.Title == "Открыть счёт")
                            {
                                window.Close();
                            }
                        }
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
                        if (window.Title == "Открыть счёт")
                        {
                            window.Close();
                        }
                    }
                });
            }
        }
    }
}
