using HomeWork_17_WPF.Services;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace HomeWork_17_WPF.Messages
{
    class Message
    {
        static List<MessageParam> messageParams = new List<MessageParam>();
        static bool close;
        
        public static void SendTo(MessageParam mes)
        {
            switch(mes.MesType)
            {
                case MessageType.AddAccount:
                case MessageType.CloseAccount:
                case MessageType.MoveMoney:
                case MessageType.AddDepositNoCapitalize:
                case MessageType.AddDepositCapitalize:
                case MessageType.RateView:
                    messageParams.Add(mes);
                    break;
                case MessageType.Save:
                    if (!close)
                        SerializeMessageList(messageParams, "_listMessage.xml");
                    close = true;
                    break;
            }
        }

        private static void SerializeMessageList(List<MessageParam> messageParams, string path)
        {
            // Создаем сериализатор на основе указанного типа 
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<MessageParam>));

            // Создаем поток для сохранения данных
            Stream fStream = new FileStream(path, FileMode.Create, FileAccess.Write);

            // Запускаем процесс сериализации
            xmlSerializer.Serialize(fStream, messageParams);

            // Закрываем поток
            fStream.Close();
        }

        static List<MessageParam> DeserializeWorkerList(string path)
        {
            List<MessageParam> tempWorkerCol = new List<MessageParam>();
            // Создаем сериализатор на основе указанного типа 
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<MessageParam>));

            // Создаем поток для чтения данных
            Stream fStream = new FileStream(path, FileMode.Open, FileAccess.Read);

            // Запускаем процесс десериализации
            tempWorkerCol = xmlSerializer.Deserialize(fStream) as List<MessageParam>;

            // Закрываем поток
            fStream.Close();

            // Возвращаем результат
            return tempWorkerCol;
        }
    }
}
