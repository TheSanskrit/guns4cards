using System;
using System.Collections.Generic;
using System.Text;

namespace D4C
{
    public enum Rarity
    {
        Mainstream = 0,
        Normal = 1,
        Rare = 2,
        Special = 3,
        Legendary = 4,
        Mythycal = 5,
    }

    public class Card
    {
        public string Title { get; }
        public Rarity Rarity { get; }

        public string Description { get; }

        public int ID { get; }

        public Card(string title, Rarity rarity, string description, int iD)
        {
            Title = title;
            Rarity = rarity;
            Description = description;
            ID = iD;
        }
    }

    public class Cards
    {
        public static List<Card> cardsList = new List<Card>
        {
            new Card("Табельный ПМ", Rarity.Mainstream, "Верный друг милиции", 001),
            new Card("Макет Калаша", Rarity.Mainstream, "Был разобран и собран миллионом школьников", 002),
            new Card("Самопал", Rarity.Mainstream, "Шанс поражения противника - 50%. Шанс поражения стрелка - 100%", 003),
            new Card("ИЖ-22", Rarity.Mainstream, "Гроза воробьев и пивных банок", 004),
            new Card("Дедушкино ружье", Rarity.Normal, "Пережило две мировых войны, и кажется переживет тебя", 101),
            new Card("Глок 'Ядерная зима'", Rarity.Normal, "Случайная мазня серой краской = искусство", 102),
            new Card("Двойные Беретты", Rarity.Normal, "Куча пуль и куча устрашения, жаль, что мало попаданий", 103),
            new Card("Zip .22", Rarity.Rare, "Такой плохой, но такой редкий....", 201),
            new Card("Золотой Дигл", Rarity.Rare, "С такого стреляли в Хищника!", 202),
            new Card("М4А1 Сайрекс", Rarity.Rare, "Добавляет очки стиля и точности", 203),
            new Card("Золотой Люгер", Rarity.Special, "Говорят, с него стрелял сам кайзер!", 301),
            new Card("AWP без прицела", Rarity.Special, "Лучше стрелять в прыжке", 302),
            new Card("Smith & Meth-son", Rarity.Legendary, "Это..... Пистолет?", 401),
            new Card("Кольт Патерсон", Rarity.Mythycal, "Самый редкий револьвер в мире!!!", 501),
            new Card("Водяной пистолет", Rarity.Mainstream, "В него можно залить не только воду...)", 005),
            new Card("Степлер-ган", Rarity.Mainstream, "В Англии забанят", 006),
            new Card("Тек-9 Авто", Rarity.Normal, "Кустарные поделки - это не игрушки", 104),
            new Card("Наган", Rarity.Normal, "Отдельная история в каждой потёртости", 105),
            new Card("Газовик под .22", Rarity.Mainstream, "Не забудьте надеть на ствол бутылку", 007),
            new Card("АК-74М", Rarity.Normal, "Бессмертная классика!", 106),
            new Card("Колибри 2.7", Rarity.Mainstream, "Как комарик укусит", 008),
            new Card("МП5-СД", Rarity.Rare, "SASно и опасно", 204),
            new Card("vz. 61 Скорпион", Rarity.Rare, "Товарищ, поедим-те ка в отделение", 205),
            new Card("АК-47 тип 1", Rarity.Special, "Дед дедов", 303),
            new Card("FN 2000", Rarity.Special, "Автомат из будущего", 304),
            new Card("Реквием", Rarity.Legendary, "С такого можно и кита завалить", 402)
        };

        //public static Dictionary<string, string> rarityEmojies = new Dictionary<string, string>
        //{
        //    { "mainEmojie", "😒"},
        //    { "normEmojie", "🥱" },
        //    { "rareEmojie", "😏" },
        //    { "specEmojie", "😎" },
        //    { "legEmojie", "🤩" },
        //    { "mythEmojie", "🤯" },
        //    { "gray", "🩶" },
        //    { "white", "🤍" },
        //    { "green", "💚" },
        //    { "blue", "💙" },
        //    { "orange", "🧡" },
        //    { "red", "❤️" }
        //};
        public static List<string> rarityEmojies = new List<string>
        {
            { "😒" },
            { "\U0001f971" },
            { "😏" },
            { "😎" },
            { "\U0001f929" },
            { "\U0001f92f" }
        };
        public static List<string> rarityHearts = new List<string>
        {
            { "\U0001fa76" },
            { "\U0001f90d" },
            { "💚" },
            { "💙" },
            { "\U0001f9e1" },
            { "❤️" }
        };

        public static int[] scoreRewards = new int[] { 100, 250, 500, 1000, 2500, 10000 };
        public static int[] coinsRewards = new int[] { 1, 2, 5, 10, 25, 100 };
    }
}
