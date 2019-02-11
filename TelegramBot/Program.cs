using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using ApiAiSDK;
using ApiAiSDK.Model;


namespace TelegramBot
{
    class Program
    {
        static TelegramBotClient Bot;
        static ApiAi apiAi;
        
        static void Main(string[] args)
        {
            
            Bot = new TelegramBotClient("766420028:AAGr1gBAmtGpRJa44hfMFSb1p9Bqe3jT3OA");
            AIConfiguration config = new AIConfiguration("cd9fdb29be96484f90caf9315f49f65e", SupportedLanguage.Russian);
            apiAi = new ApiAi(config);

            Bot.OnMessage += BotOnMessageReceived;
            Bot.OnCallbackQuery += BotOnCallBackQueryReceived;
            var me = Bot.GetMeAsync().Result;

            Console.WriteLine(me.FirstName);
            Bot.StartReceiving();
            Console.ReadLine();
            Bot.StopReceiving();
        }

        private static async void BotOnCallBackQueryReceived(object sender, Telegram.Bot.Args.CallbackQueryEventArgs e)
        {
            string buttonText = e.CallbackQuery.Data;
            string name = $"{e.CallbackQuery.From.FirstName} {e.CallbackQuery.From.LastName}";
            Console.WriteLine($"{name} нажал кнопку {buttonText}");

            if (buttonText == "Картинка")
            {
                await Bot.SendTextMessageAsync(e.CallbackQuery.From.Id, "https://pp.userapi.com/c850432/v850432120/bbcf/Ewn3iF2WE4c.jpg");
            }
            else if (buttonText == "Видео")
            {
                await Bot.SendTextMessageAsync(e.CallbackQuery.From.Id, "https://www.youtube.com/watch?v=PEKN8NtBDQ0");
            }

            await Bot.AnswerCallbackQueryAsync(e.CallbackQuery.Id, $"Вы нажали кнопку {buttonText}");

        }

        private static async void BotOnMessageReceived(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            var message = e.Message;
            if (message == null || message.Type != MessageType.Text)
                return;
            string name = $"{message.From.FirstName} {message.From.LastName}";
            Console.WriteLine($"{name} отправил сообщение: {message.Text}");
            switch(message.Text)
            {
                case "/start":
                    string text =
                        @"Список команд:
                        /start - запуск бота
                        /menu - вывод меню
                        /keyboard - ввод с клавиатуры";
                    await Bot.SendTextMessageAsync(message.From.Id, text);
                    break;
                case "/menu":
                    var inlineKeyboard = new InlineKeyboardMarkup(new[]{
                        new[]
                        {
                            InlineKeyboardButton.WithUrl("VK ILUSHA","https://vk.com/special0ps"),
                            InlineKeyboardButton.WithUrl("VK ANDRUSHA", "https://vk.com/id231468284")
                        },
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("Картинка"),
                            InlineKeyboardButton.WithCallbackData("Видео")
                        }
                    });
                    await Bot.SendTextMessageAsync(message.From.Id, "Выберите пункт меню", replyMarkup: inlineKeyboard);
                    break;
                case "/keyboard":
                    var replyKeyboard = new ReplyKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            new KeyboardButton("Привет"),
                            new KeyboardButton("Как дела?")
                        },
                        new[]
                        {
                            new KeyboardButton("Контакт") {RequestContact = true},
                            new KeyboardButton("Геолокация") {RequestLocation = true}
                        }
                    });
                    await Bot.SendTextMessageAsync(message.Chat.Id, "Сообщение", replyMarkup: replyKeyboard);
                    break;
                default:
                    var response = apiAi.TextRequest(message.Text);
                    string answer = response.Result.Fulfillment.Speech;
                    if (answer == "")
                        answer = "Я тебя не понимаю:(";
                    await Bot.SendTextMessageAsync(message.From.Id, answer);
                    break;
            }
        }
    }
}
