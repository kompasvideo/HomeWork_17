using HomeWork_17_WPF.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HomeWork_17_WPF
{
    /// <summary>
    /// Класс с расширяющим методом
    /// Расчёт % в рублях за месяц
    /// </summary>
    public static class StaticExt
    {
        static public byte[] daysOnMonth = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };


        /// <summary>
        /// Расщиряющий метод вместо Deposit.GetSumRate(uint Money)
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static string[] GetSumRateExt(this DataRow client)
        {
            string[] sumStr = new string[12];
            try
            {
                double[] sum = new double[12];
                double[] sumPlusDeposit = new double[12];
                double money = (int)client[2];
                double sumRate = (int)client[2] * (double)client[7] / 100 / 365;
                for (int i = 0; i < 12; i++)
                {
                    sum[i] = sumRate * daysOnMonth[i];
                    money += sum[i];
                    sumPlusDeposit[i] = money;
                    sumStr[i] = string.Format($"{sum[i]:f2} руб   {sumPlusDeposit[i]:f2} руб");
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
            return sumStr;
        }

    }
}
