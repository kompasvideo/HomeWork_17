using System;

namespace HomeWork_17_WPF.Model
{
    public class VIPClient : Client
    {
        /// <summary>
        /// Хранит в тексовом виде к какому департаменту относится клиент
        /// </summary>
        public override string Status
        {
            get
            {
                return Const.VIPName;
            }
        }

        
        public VIPClient() : base($"{Const.VIPName} - {Guid.NewGuid().ToString().Substring(0, 5)}")
        {
        }
        public VIPClient(string Name, uint Money) : base(Name, Money)
        {
        }

        /// <summary>
        /// Возвращяет enum к какому департаменту относится клиент
        /// </summary>
        /// <returns></returns>
        public override BankDepartment BankDepartmentProp
        {
            get
            {
                return BankDepartment.VIPDepartment;
            }
        }
    }
}
