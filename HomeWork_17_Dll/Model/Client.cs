using System;

namespace HomeWork_17_WPF.Model
{
    public abstract class Client
    {
        /// <summary>
        /// Имя клиента
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Сумма на счёте клиента
        /// </summary>
        public uint Money { get; set; }
        /// <summary>
        /// Хранит в тексовом виде к какому департаменту относится клиент
        /// </summary>
        public abstract string Status { get; }
        /// <summary>
        /// Вклад 
        /// </summary>
        public Deposit DepositClient { get; set; }
        /// <summary>
        /// Название вклада
        /// </summary>
        public string DepositClientStr { get; set; }
        /// <summary>
        /// Число дней в месяц
        /// </summary>
        static Random rnd = new Random();

        
        public Client(string name)
        {
            Name = name;
            Money = (uint)rnd.Next(0, 10000);
            DepositClientStr = "Нет";
        }
        public Client(string name, uint Money)
        {
            Name = name;
            this.Money = Money;
            DepositClientStr = "Нет";
        }

        /// <summary>
        /// Возвращяет enum к какому департаменту относится клиент
        /// </summary>
        /// <returns></returns>
        public abstract BankDepartment BankDepartmentProp { get; }

        /// <summary>
        /// Перегрузка оператора + для Client
        /// </summary>
        /// <param name="client"></param>
        /// <param name="chislo"></param>
        /// <returns></returns>
        public static Client operator + (Client client, uint chislo)
        {
            Client newClient = (Client)client.MemberwiseClone();
            newClient.Money += chislo;
            return newClient;
        }

        /// <summary>
        /// Перегрузка оператора - для Client
        /// </summary>
        /// <param name="client"></param>
        /// <param name="chislo"></param>
        /// <returns></returns>
        public static Client operator -(Client client, uint chislo)
        {
            Client newClient = (Client)client.MemberwiseClone();
            newClient.Money -= chislo;
            return newClient;
        }
    }
}
