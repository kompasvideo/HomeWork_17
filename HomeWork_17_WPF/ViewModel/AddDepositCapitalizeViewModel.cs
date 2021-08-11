using DevExpress.Mvvm;
using HomeWork_17_WPF.Model;
using System.Windows.Input;

namespace HomeWork_17_WPF.ViewModel
{
    class AddDepositCapitalizeViewModel : ViewModelBase
    {
        public static DepositPlusCapitalize DepositV { get; set; }
        static BankDepartment bankDepartment;

        public AddDepositCapitalizeViewModel()
        {
            
        }
        /// <summary>
        /// Принимает аргумент BankDepartment pBankDepartment
        /// </summary>
        /// <param name="pBankDepartment"></param>
        public static void SetBankDepartment(BankDepartment pBankDepartment)
        {
            DepositV = new DepositPlusCapitalize();
            bankDepartment = pBankDepartment;
            switch (bankDepartment)
            {
                case BankDepartment.BusinessDepartment:
                    DepositV.InterestRate = 12;
                    break;
                case BankDepartment.PersonalDepartment:
                    DepositV.InterestRate = 24;
                    break;
                case BankDepartment.VIPDepartment:
                    DepositV.InterestRate = 36;
                    break;
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
                    Messenger.Default.Send(DepositV);

                    foreach (System.Windows.Window window in System.Windows.Application.Current.Windows)
                    {
                        if (window.Title == "Открыть вклад с капитализацией %")
                        {
                            window.Close();
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
                        if (window.Title == "Открыть вклад с капитализацией %")
                        {
                            window.Close();
                        }
                    }
                });
            }
        }
    }
}
