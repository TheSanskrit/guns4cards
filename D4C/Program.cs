using D4C;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

var botClient = new TelegramBotClient("8727863080:AAHTC23z3AREtFpaJ1bgFgCYVC8xrwj7Zyw");

using var cts = new CancellationTokenSource();


var receiverOptions = new ReceiverOptions
{
    AllowedUpdates = Array.Empty<UpdateType>()
};

botClient.StartReceiving(
    updateHandler: HandleUpdateAsync,
    errorHandler: HandleErrorAsync,
    receiverOptions: receiverOptions,
    cancellationToken: cts.Token 
);

DataBase.Connect();

Console.WriteLine("Бот запущен");
await Task.Delay(-1);

static string EscapeMarkdown(string text)
{
    string[] charactersToEscape = { "_", "*", "[", "]", "(", ")", "~", "`", ">", "#", "+", "-", "=", "|", "{", "}", ".", "!" };
    foreach (var character in charactersToEscape)
    {
        text = text.Replace(character, "\\" + character);
    }
    return text;
}

static string RollReply(Rarity rarity)
{
    string message = "";

    message = $"";
    switch ((int)rarity)
    {
        case 0: message += $"<b><i>{EntComs.rollQuotes[0 + EntComs.rnd.Next(0, 2)]}</i></b>\n\n<b>{EntComs.rollQuotes[12 + EntComs.rnd.Next(0, 2)]}</b>"; break ;
        case 1: message += $"<b><i>{EntComs.rollQuotes[0 + EntComs.rnd.Next(0, 5)]}</i></b>\n\n<b>{EntComs.rollQuotes[12 + EntComs.rnd.Next(0, 5)]}</b>"; break;
        case 2: message += $"<b><i>{EntComs.rollQuotes[3 + EntComs.rnd.Next(0, 2)]}</i></b>\n\n<b>{EntComs.rollQuotes[15 + EntComs.rnd.Next(0, 2)]}</b>"; break; ;
        case 3: message += $"<b><i>{EntComs.rollQuotes[6 + EntComs.rnd.Next(0, 2)]}</i></b>\n\n<b>{EntComs.rollQuotes[18 + EntComs.rnd.Next(0, 2)]}</b>"; break;;
        case 4: message += $"<b><i>{EntComs.rollQuotes[6 + EntComs.rnd.Next(0, 2)]}</i></b>\n\n<b>{EntComs.rollQuotes[18 + EntComs.rnd.Next(0, 2)]}</b>"; break; ;
        case 5: message += $"<b><i>{EntComs.rollQuotes[9 + EntComs.rnd.Next(0, 2)]}</i></b>\n\n<b>{EntComs.rollQuotes[21 + EntComs.rnd.Next(0, 2)]}</b>"; break; ;
    }

    return message;
}

static string GetGun(long userId, Rarity rarity, Card card)
{
    string message = "";
    switch((int)rarity)
    {
        case 0: message = $"<i>{EntComs.rollQuotes[24 + EntComs.rnd.Next(0, 1)]}</i>"; break;
        case 1: message = $"<i>{EntComs.rollQuotes[26 + EntComs.rnd.Next(0, 1)]}</i>"; break;
        case 2: message = $"<i>{EntComs.rollQuotes[28 + EntComs.rnd.Next(0, 1)]}</i>"; break;
        case 3: message = $"<i> {EntComs.rollQuotes[30 + EntComs.rnd.Next(0, 1)]}</i>"; break;
        case 4: message = $"<i> {EntComs.rollQuotes[32 + EntComs.rnd.Next(0, 1)]}</i>"; break;
        case 5: message = $"<i> {EntComs.rollQuotes[34 + EntComs.rnd.Next(0, 1)]}</i>"; break;
    }

    message += $"\n\nВы получили: <b>{card.Title}</b>!!!" +
               $"\n\"<i>{card.Description}</i>\"" +
               $"\n\n<i>Редкость</i>: <b>{Cards.rarityHearts[(int)card.Rarity]}{card.Rarity}{Cards.rarityHearts[(int)card.Rarity]}</b>" +
               $"\n<i>Кол-во стволов у пользователя</i>: <b>{DataBase.GetCardsAmount(userId)}/{Cards.cardsList.Count()}</b>" +
               $"\n<i>Вы получили</i>: <b>{Cards.scoreRewards[(int)rarity]} очков, {Cards.coinsRewards[(int)rarity]} монет</b>";

    return message;
}











async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken token)
{

    var time = DateTime.Now;
    string timeText = $"{time.Month}.{time.Day} {time.Hour}:{time.Minute}";

    if (update.CallbackQuery != null && update.CallbackQuery.Data.StartsWith("open_gun_"))
    {
        var rarityValue = int.Parse(update.CallbackQuery.Data.Split('_')[2]);
        var rarity = (Rarity)rarityValue;
        var callback = update.CallbackQuery;
        int uTime = (int)((DateTimeOffset)time).ToUnixTimeSeconds();

        var bonusKey = new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData("Бонусный ствол", "get_bonus"));

        await bot.EditMessageText(
            chatId: callback.Message.Chat.Id,
            messageId: callback.Message.MessageId,
            text: GetGun(
                callback.From.Id,
                rarity,
                EntComs.GiveCard(callback.From.Id, rarity, uTime, false)
            ),
            parseMode: ParseMode.Html,
            replyMarkup: bonusKey

        );

        await bot.AnswerCallbackQuery(update.CallbackQuery.Id);
    }

    else if(update.CallbackQuery != null && update.CallbackQuery.Data.StartsWith("get_bonus"))
    {
        var callback = update.CallbackQuery;
        long userId = callback.From.Id;
        //bool subscribed = await IsUserSubscribed(userId, "@deadprogrammist");
        bool subscribed = true; // для тестов, убрать потом
        Console.WriteLine("yay1");
        if( subscribed )
        {
            try
            {
                int uTime = (int)((DateTimeOffset)time).ToUnixTimeSeconds();
                var cooldown = DataBase.Cooldown(userId, uTime, 3 * 3600);
                Console.WriteLine("yay2");

                if (cooldown.Item2)
                {
                    Rarity rarity = EntComs.RandomCard();
                    Card card = EntComs.GiveCard(userId, rarity, uTime, true);
                    Console.WriteLine("yay3");
                    await bot.SendMessage(callback.Message.Chat.Id, GetGun(userId, rarity, card), parseMode: ParseMode.Html);
                    Console.WriteLine($"Забрали пушку,  {timeText},     {callback.From.FirstName} {callback.From.LastName}");
                    await bot.AnswerCallbackQuery(update.CallbackQuery.Id);
                }
                else
                {
                    Console.WriteLine("yay4");
                    DateTime remTime = DateTimeOffset.FromUnixTimeSeconds(cooldown.Item4).DateTime;
                    await bot.SendMessage(callback.Message.Chat.Id, $"\U0001fae4Пока товар не подвезли\U0001fae4\n\nСледующая бонусная партия через: {remTime.ToString("HH:mm:ss")}", replyParameters: update.Message.Id);
                    Console.WriteLine("yay5");
                    await bot.AnswerCallbackQuery(update.CallbackQuery.Id);
                }
            }
            catch (Exception ex) { Console.WriteLine($"Ошибка при выдаче бонусного ствола пользователю {userId}: {ex.Message}"); }
        }
        else
        {
            await bot.SendMessage(callback.Message.Chat.Id, "Ага, вы кажется не подписаны на канал разработчика бота.\n\nПожалуйста, перейдите в профиль бота" +
                " и подпишитесь на канал по ссылке. Это позволит вам получать бонусную карточку каждые 8 часов");
            await bot.AnswerCallbackQuery(update.CallbackQuery.Id);
        }
    }



    if (update.Message?.Text == null)
        return;

    var text = update.Message.Text;
    var normalized = text.Trim().ToLower();
    var chatId = update.Message.Chat.Id;
    var userID = update.Message.From.Id;
    var userName = $"{update.Message.From.FirstName} {update.Message.From.LastName}";

    if (!DataBase.HasID(userID)) { DataBase.AddUser(userID, userName); }

    if (normalized == "!я" || normalized.StartsWith("/gunprofile"))
    {
         string message = $"Пользователь: <i>{EscapeMarkdown(DataBase.GetStats(userID).name)}</i>" +
                 $"\n\n\n\nКол-во пушек у пользователя: <b>{DataBase.GetCardsAmount(userID)}/{Cards.cardsList.Count()}</b>" +
                 $"\n\nШирпотреб стволов: {Cards.rarityHearts[0]}<b>{DataBase.GetRarityCards(userID).mainstream}/{Cards.cardsList.Where(c => c.Rarity == Rarity.Mainstream).Count()}</b>{Cards.rarityHearts[0]}" +
                 $"\n\nНормальных стволов: {Cards.rarityHearts[1]}<b>{DataBase.GetRarityCards(userID).normal}/{Cards.cardsList.Where(c => c.Rarity == Rarity.Normal).Count()}</b>{Cards.rarityHearts[1]}" +
                 $"\n\nРедких стволов: {Cards.rarityHearts[2]}<b>{DataBase.GetRarityCards(userID).rare}/{Cards.cardsList.Where(c => c.Rarity == Rarity.Rare).Count()}{Cards.rarityHearts[2]}</b>" +
                 $"\n\nОсобенных стволов: {Cards.rarityHearts[3]}<b>{DataBase.GetRarityCards(userID).special}/{Cards.cardsList.Where(c => c.Rarity == Rarity.Special).Count()}</b>{Cards.rarityHearts[3]}" +
                 $"\n\nЛегендарных стволов: {Cards.rarityHearts[4]}<b>{DataBase.GetRarityCards(userID).leg}/{Cards.cardsList.Where(c => c.Rarity == Rarity.Legendary).Count()}</b>{Cards.rarityHearts[4]}" +
                 $"\n\nМифических стволов: {Cards.rarityHearts[5]}<b>{DataBase.GetRarityCards(userID).myth}/{Cards.cardsList.Where(c => c.Rarity == Rarity.Mythycal).Count()}</b>{Cards.rarityHearts[5]}" +
                 $"\n\nСчёт: <i>{DataBase.GetStats(userID).score}</i>" +
                 $"\nМонеты: <i>{DataBase.GetStats(userID).coins}</i>";
        await bot.SendMessage(chatId, message, parseMode: ParseMode.Html); Console.WriteLine($"Обратились к данным пользователя {timeText}");
    }



    else if(normalized.StartsWith("!имя "))
    {
        string newNick = normalized.Substring(5).Trim();

        DataBase.ChangeName(newNick, userID);
        await bot.SendMessage(chatId, "Ваш ник изменен!");

    }



    else if (normalized.StartsWith("/scoretop") || normalized == "!очкитоп")
    {
        int[] scores = DataBase.Top().score; string[] names = DataBase.Top().nick;

        string message = $"<b>Топ-10 стрелков:</b>\n\n\n";
        for (int i = 0; i < scores.Length; i++)
        {
            message += $"<b>{i + 1}. {names[i]}</b> - <i>{scores[i]}</i>\n";
        }

        await bot.SendMessage(chatId, message, parseMode: ParseMode.Html);
    }


    else if (normalized.StartsWith("/coinstop") || normalized == "!монетытоп")
    {
        int[] coins = DataBase.Top().coins; string[] names = DataBase.Top().nick;

        string message = $"<b>Топ-10 толстосумов:</b>\n\n\n";
        for (int i = 0; i < coins.Length; i++)
        {
            message += $"<b>{i + 1}. {EscapeMarkdown(names[i])}</b> - <i>{coins[i]}</i>\n";
        }

        await bot.SendMessage(chatId, message, parseMode: ParseMode.Html);
    }



    else if (normalized == "!мои стволы" || normalized.StartsWith("/guncards"))
    {
        if (update.Message.Chat.Type == ChatType.Private)
        {

        }
    }



    //else if(normalized == "!пушка" || normalized.StartsWith("/gun"))
    //{
    //    int uTime = (int)((DateTimeOffset)time).ToUnixTimeSeconds();
    //        if (DataBase.Cooldown(userID, uTime, 3 * 3600).Item1)
    //        {
    //            Rarity rarity = EntComs.RandomCard();
    //            Card card = EntComs.GiveCard(userID, rarity, uTime);

    //            string message = $"Вы получили\\.\\.\\.\n{Cards.rarityEmojies[(int)card.Rarity]}*{EscapeMarkdown(card.Title)}*{Cards.rarityEmojies[(int)card.Rarity]}\\!\\!\\! \n" +
    //                             $"\"_{EscapeMarkdown(card.Description)}_\"\n\n\n" +
    //                             $"Редкость: {Cards.rarityHearts[(int)card.Rarity]}*{EscapeMarkdown(card.Rarity.ToString())}*{Cards.rarityHearts[(int)card.Rarity]}\n" +
    //                             $"Кол\\-во стволов у пользователя: *{DataBase.GetCardsAmount(userID)}/{Cards.cardsList.Count()}*\n" +
    //                             $"Вы получили: *{Cards.scoreRewards[(int)rarity]} очков, {Cards.coinsRewards[(int)rarity]} монет*";

    //            await bot.SendMessage(chatId, message, parseMode: ParseMode.MarkdownV2, replyParameters: update.Message.Id);
    //            Console.WriteLine($"Забрали пушку,  {timeText},     {userName}");
    //        }
    //        else
    //        {
    //            DateTime remTime = DateTimeOffset.FromUnixTimeSeconds(DataBase.Cooldown(userID, uTime, 3 * 3600).Item2).DateTime;
    //            await bot.SendMessage(chatId, $"\U0001fae4Пока товар не подвезли\U0001fae4\n\nСледующая партия через: {remTime.ToString("HH:mm:ss")}", replyParameters: update.Message.Id);
    //        }

    //}
    else if (normalized == "!пушка" || normalized.StartsWith("/gun"))
    {
        int uTime = (int)((DateTimeOffset)time).ToUnixTimeSeconds();
        var cooldownResult = DataBase.Cooldown(userID, uTime, 3 * 3600);
        if (cooldownResult.Item1)
        {
            var rarity = EntComs.RandomCard();
            var openBox = new InlineKeyboardMarkup(
                InlineKeyboardButton.WithCallbackData(
                    "Открыть",
                    $"open_gun_{(int)rarity}"
                )
            );
            await bot.SendMessage(chatId, RollReply(rarity), parseMode: ParseMode.Html, replyParameters: update.Message.Id, replyMarkup: openBox);
            Console.WriteLine($"Забрали пушку,  {timeText},     {userName}");
        }
        else
        {
            DateTime remTime = DateTimeOffset.FromUnixTimeSeconds(cooldownResult.Item3).DateTime;
            await bot.SendMessage(chatId, $"\U0001fae4Пока товар не подвезли\U0001fae4\n\nСледующая партия через: {remTime.ToString("HH:mm:ss")}", replyParameters: update.Message.Id);
        }

    }






    else if(EntComs.fixedResponces.ContainsKey(normalized))
    {
        await bot.SendMessage(chatId, $"{EntComs.fixedResponces[normalized]}");
        Console.WriteLine($"Вызвали фиксированный ответ, {timeText}");
    }
    //else
    //{
    //    await bot.SendMessage(chatId, "Ага, получен неожиданный неправильный ввод. Dazed and Confused, but trying to continue");
    //    Console.WriteLine("Неправильный ввод");
    //}

}

Task HandleErrorAsync(ITelegramBotClient bot, Exception exception, CancellationToken token)
{
    Console.WriteLine(exception.ToString());
    return Task.CompletedTask;
}




async Task<bool> IsUserSubscribed(long userId, string channelId)
{
    try
    {
        ChatMember member = await botClient.GetChatMember(channelId, userId);

        return member.Status switch
        {
            ChatMemberStatus.Creator => true,
            ChatMemberStatus.Administrator => true,
            ChatMemberStatus.Member => true,
            _ => false
        };
    }
    catch (Exception)
    {
        return false;
    }
}