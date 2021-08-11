using System;

namespace HomeWork_17_WPF.Model
{
    public class DepositPlusCapitalize : Deposit
    {
        public DepositPlusCapitalize() : base()
        {
        }
        public DepositPlusCapitalize(DateTime dateBegin, float interestRate = 12f, uint days = 365) :
            base(dateBegin, interestRate, days)
        {
        }

        /// <summary>
        /// Расчёт % в рублях за месяц
        /// </summary>
        /// <param name="Money"></param>
        /// <returns>Возвращяет массив типа string - рассчёт % в рублях и суммы вклада по месяцам</returns>
        public override string[] GetSumRate(uint Money)
        {
            double[] sum = new double[12];
            double[] sumPlusDeposit = new double[12];
            double money = Money;
            double sumRate;
            string[] sumStr = new string[12];
            for (int i = 0; i < 12; i++)
            {
                sumRate = money * InterestRate / 100 / 365;
                sum[i] = sumRate * daysOnMonth[i];
                money += sum[i];
                sumPlusDeposit[i] = money;
                sumStr[i] = string.Format($"{sum[i]:f2} руб   {sumPlusDeposit[i]:f2} руб");
            }
            return sumStr;
        }
    }
}
