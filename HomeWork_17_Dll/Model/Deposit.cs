using System;

namespace HomeWork_17_WPF.Model
{
    public class Deposit
    {
        /// <summary>
        /// Процентная ставка
        /// </summary>
        public float InterestRate { get; set; }
        /// <summary>
        /// Срок открытия депозита
        /// </summary>
        public DateTime DateBegin { get; set; }
        /// <summary>
        /// Срок кредита в днях
        /// </summary>
        public uint Days { get; set; }
        public byte[] daysOnMonth = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

        public Deposit()
        {
            InterestRate = 12f;
            DateBegin = DateTime.Now;
            Days = 365;
        }
        public Deposit(DateTime dateBegin, float interestRate = 12f, uint days = 365)
        {
            InterestRate = interestRate;
            DateBegin = dateBegin;
            Days = days;
        }

        /// <summary>
        /// Расчёт % в рублях за месяц
        /// </summary>
        /// <param name="Money"></param>
        /// <returns>Возвращяет массив типа string - рассчёт % в рублях и суммы вклада по месяцам</returns>
        public virtual string[] GetSumRate(uint Money)
        {
            double[] sum = new double[12];
            double[] sumPlusDeposit = new double[12];
            double money = Money;
            double sumRate = Money * InterestRate / 100 / 365;
            string[] sumStr = new string[12];
            for (int i = 0; i < 12; i++)
            {
                sum[i] = sumRate * daysOnMonth[i];
                money += sum[i];
                sumPlusDeposit[i] = money;
                sumStr[i] = string.Format($"{sum[i]:f2} руб   {sumPlusDeposit[i]:f2} руб");
            }
            return sumStr;
        }
    }
}
