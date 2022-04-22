using System;
using System.Collections.Generic;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;

using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

using System.Threading.Tasks;
using System.Threading;
using System.Configuration;
using ParserTabletkiUa;

namespace TelBotConsole
{

    internal class Program
    {

        private static string Token { get; set; } = "5239080494:AAHdI_BH2gykGpFk486vAWVp31UGL6GLbiA";
        private static TelegramBotClient client;
        static User me;
        static  bool IsDrugFounded = false;
        static bool IsFindMode = false;


        static async Task Main(string[] args)
        {
            client = new TelegramBotClient(Token);
            client.StartReceiving();
            client.OnMessage += Client_OnMessage;

            Send();

            Console.ReadLine();


        }


        private static async void SendResponce(Message msg)
        {

            if (msg != null)
            {
                
                Find(msg);
                //if (IsDrugFounded)
                //    await client.SendTextMessageAsync(msg.Chat.Id, "Пошук завершено. Введіть наступну назву препарату, який вас цікавить:");
                //else
                    await client.SendTextMessageAsync(msg.Chat.Id, "Введіть назву препарату українською мовою:");
                IsDrugFounded = true;
                return;



            }
        }

        static List<DrugBlock> Find(Message msg)
        {
            Parser.Host = @"https://tabletki.ua/uk/";
            Parser.SearchByName(msg.Text);


            long i = 0;
            while (true)
            {
                if (Parser.IsFound)
                {
                    Parser.IsFound = false;
                    foreach (DrugBlock db in Parser.ListOfDrugsBlocks)
                    {
                        Thread.Sleep(300);
                        if (!String.IsNullOrEmpty(db.PriceString))
                        {
                            ReplyDrug(msg, db);
                        }
                    }
                    IsFindMode = false;
                    return Parser.ListOfDrugsBlocks;
                }
                i++;
                if (i > 10000000000)
                {

                    break;
                }
            }
            IsFindMode = false;
            return null;


        }


        private static async void SendDrugBlockToChat(Message msg, string text, DrugBlock db)
        {
            
            client.SendPhotoAsync(msg.Chat.Id, db.ImageSource, text, ParseMode.Html);
            IsDrugFounded = true;
        }



        private static async void ReplyDrug(Message msg, DrugBlock db)
        {

            SendDrugBlockToChat(msg, "Назва: " + db.Name + "\n"

            + "Виробник: " + db.Manufacturer + "\n"
            + "Ціна: " + db.PriceString + "\n"
            + "<a href='" + db.LinkStores+ "'>Дивитись у аптеках</a>"

             , db);



        }

        private static async void Client_OnMessage(object sender, MessageEventArgs e)
        {
            var msg = e.Message;
            if (msg != null)
            {
                SendResponce(msg);
            }
        }


        static async void Send()
        {
            client = new TelegramBotClient(Token);
            me = await client.GetMeAsync();
            Console.WriteLine($"Hello, World! I am user {me.Id} and my name is {me.FirstName}.");
        }

    }
}
