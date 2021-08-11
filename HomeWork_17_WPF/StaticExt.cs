using HomeWork_17_WPF.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeWork_17_WPF
{
    /// <summary>
    /// Класс с расширяющим методом
    /// Расчёт % в рублях за месяц
    /// </summary>
    public static class StaticExt
    {
        /// <summary>
        /// Расщиряющий метод вместо Deposit.GetSumRate(uint Money)
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static string[] GetSumRateExt(this Client client)
        {
            double[] sum = new double[12];
            double[] sumPlusDeposit = new double[12];
            double money = client.Money;
            double sumRate = client.Money * client.DepositClient.InterestRate / 100 / 365;
            string[] sumStr = new string[12];
            for (int i = 0; i < 12; i++)
            {
                sum[i] = sumRate * client.DepositClient.daysOnMonth[i];
                money += sum[i];
                sumPlusDeposit[i] = money;
                sumStr[i] = string.Format($"{sum[i]:f2} руб   {sumPlusDeposit[i]:f2} руб");
            }
            return sumStr;
        }

    }
}
