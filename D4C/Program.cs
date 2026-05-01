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
        case 0: message += $"*{ EscapeMarkdown(EntComs.rollQuotes[0 + EntComs.rnd.Next(0, 2)])}\n\n_{ EscapeMarkdown(EntComs.rollQuotes[12 + EntComs.rnd.Next(0, 2)])} _ *"; break ;
        case 1: message += $"*{EscapeMarkdown(EntComs.rollQuotes[0 + EntComs.rnd.Next(0, 5)])}\n\n_{EscapeMarkdown(EntComs.rollQuotes[12 + EntComs.rnd.Next(0, 5)])} _ *"; break;
        case 2: message += $"*{EscapeMarkdown(EntComs.rollQuotes[3 + EntComs.rnd.Next(0, 2)])}\n\n_{EscapeMarkdown(EntComs.rollQuotes[15 + EntComs.rnd.Next(0, 2)])} _ *"; break; ;
        case 3: message += $"*{EscapeMarkdown(EntComs.rollQuotes[6 + EntComs.rnd.Next(0, 2)])}\n\n_{EscapeMarkdown(EntComs.rollQuotes[18 + EntComs.rnd.Next(0, 2)])} _ *"; break;;
        case 4: message += $"*{EscapeMarkdown(EntComs.rollQuotes[6 + EntComs.rnd.Next(0, 2)])}\n\n_{EscapeMarkdown(EntComs.rollQuotes[18 + EntComs.rnd.Next(0, 2)])} _ *"; break; ;
        case 5: message += $"*{EscapeMarkdown(EntComs.rollQuotes[9 + EntComs.rnd.Next(0, 2)])}\n\n_{EscapeMarkdown(EntComs.rollQuotes[21 + EntComs.rnd.Next(0, 2)])} _ *"; break; ;
    }

    return message;
}

static string GetGun(long userId, Rarity rarity, Card card)
{
    string message = "";
    switch((int)rarity)
    {
        case 0: message = $"{EscapeMarkdown(EntComs.rollQuotes[24 + EntComs.rnd.Next(0, 1)])}"; break;
        case 1: message = $"{EscapeMarkdown(EntComs.rollQuotes[26 + EntComs.rnd.Next(0, 1)])}"; break;
        case 2: message = $"{EscapeMarkdown(EntComs.rollQuotes[28 + EntComs.rnd.Next(0, 1)])}"; break;
        case 3: message = $"{EscapeMarkdown(EntComs.rollQuotes[30 + EntComs.rnd.Next(0, 1)])}"; break;
        case 4: message = $"{EscapeMarkdown(EntComs.rollQuotes[32 + EntComs.rnd.Next(0, 1)])}"; break;
        case 5: message = $"{EscapeMarkdown(EntComs.rollQuotes[34 + EntComs.rnd.Next(0, 1)])}"; break;
    }

    message += $"\n\nВы получили: {EscapeMarkdown(card.Title)}\\!\\!\\!" +
               $"\n-\"{EscapeMarkdown(card.Description)}\"_" +
               $"\n\nРедкость: *{Cards.rarityHearts[(int)card.Rarity]}{card.Rarity}{Cards.rarityHearts[(int)card.Rarity]}*" +
               $"\nКол\\-во стволов у пользователя: *{DataBase.GetCardsAmount(userId)}/{Cards.cardsList.Count()}*" +
               $"\nВы получили: *{Cards.scoreRewards[(int)rarity]} очков, {Cards.coinsRewards[(int)rarity]} монет*";

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

        await bot.EditMessageText(
            chatId: callback.Message.Chat.Id,
            messageId: callback.Message.MessageId,
            text: GetGun(
                callback.From.Id,
                rarity,
                EntComs.GiveCard(callback.From.Id, rarity, 100)
            ),
            parseMode: ParseMode.MarkdownV2,
            replyMarkup: null

        );

        await bot.AnswerCallbackQuery(update.CallbackQuery.Id);
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
         string message = $"Пользователь: _{EscapeMarkdown(DataBase.GetStats(userID).name)}_" +
                 $"\n\n\n\nКол\\-во пушек у пользователя: *{DataBase.GetCardsAmount(userID)}/{Cards.cardsList.Count()}*" +
                 $"\n\nШирпотреб стволов: {Cards.rarityHearts[0]}*{DataBase.GetRarityCards(userID).mainstream}/{Cards.cardsList.Where(c => c.Rarity == Rarity.Mainstream).Count()}*{Cards.rarityHearts[0]}" +
                 $"\n\nНормальных стволов: {Cards.rarityHearts[1]}*{DataBase.GetRarityCards(userID).normal}/{Cards.cardsList.Where(c => c.Rarity == Rarity.Normal).Count()}*{Cards.rarityHearts[1]}" +
                 $"\n\nРедких стволов: {Cards.rarityHearts[2]}*{DataBase.GetRarityCards(userID).rare}/{Cards.cardsList.Where(c => c.Rarity == Rarity.Rare).Count()}{Cards.rarityHearts[2]}*" +
                 $"\n\nОсобенных стволов: {Cards.rarityHearts[3]}*{DataBase.GetRarityCards(userID).special}/{Cards.cardsList.Where(c => c.Rarity == Rarity.Special).Count()}*{Cards.rarityHearts[3]}" +
                 $"\n\nЛегендарных стволов: {Cards.rarityHearts[4]}*{DataBase.GetRarityCards(userID).leg}/{Cards.cardsList.Where(c => c.Rarity == Rarity.Legendary).Count()}*{Cards.rarityHearts[4]}" +
                 $"\n\nМифических стволов: {Cards.rarityHearts[5]}*{DataBase.GetRarityCards(userID).myth}/{Cards.cardsList.Where(c => c.Rarity == Rarity.Mythycal).Count()}*{Cards.rarityHearts[5]}" +
                 $"\n\nСчёт: _{DataBase.GetStats(userID).score}_" +
                 $"\nМонеты: _{DataBase.GetStats(userID).coins}_";
        await bot.SendMessage(chatId, message, parseMode: ParseMode.MarkdownV2); Console.WriteLine($"Обратились к данным пользователя {timeText}");
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

        string message = $"*Топ\\-10 стрелков:*\n\n\n";
        for (int i = 0; i < scores.Length; i++)
        {
            message += $"*{i + 1}\\. {EscapeMarkdown(names[i])}* \\- _{scores[i]}_\n";
        }

        await bot.SendMessage(chatId, message, parseMode: ParseMode.MarkdownV2);
    }


    else if (normalized.StartsWith("/coinstop") || normalized == "!монетытоп")
    {
        int[] coins = DataBase.Top().coins; string[] names = DataBase.Top().nick;

        string message = $"*Топ\\-10 толстосумов:*\n\n\n";
        for (int i = 0; i < coins.Length; i++)
        {
            message += $"*{i + 1}\\. {EscapeMarkdown(names[i])}* \\- _{coins[i]}_\n";
        }

        await bot.SendMessage(chatId, message, parseMode: ParseMode.MarkdownV2);
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
        if (DataBase.Cooldown(userID, uTime, 3 * 3600).Item1)
        {
            var rarity = EntComs.RandomCard();
            var openBox = new InlineKeyboardMarkup(
                InlineKeyboardButton.WithCallbackData(
                    "Открыть",
                    $"open_gun_{(int)rarity}"
                )
            );
            await bot.SendMessage(chatId, RollReply(rarity), parseMode: ParseMode.MarkdownV2, replyParameters: update.Message.Id, replyMarkup: openBox);
            Console.WriteLine($"Забрали пушку,  {timeText},     {userName}");
        }
        else
        {
            DateTime remTime = DateTimeOffset.FromUnixTimeSeconds(DataBase.Cooldown(userID, uTime, 3 * 3600).Item2).DateTime;
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