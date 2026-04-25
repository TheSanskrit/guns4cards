using System;
using System.Collections.Generic;
using System.Text;

namespace D4C
{


    public static class EntComs
    {
        static Random rnd = new Random();
        static Random rnd2 = new Random();

        public static Dictionary<string, string> fixedResponces = new Dictionary<string, string>()
        {
            {"пиу", "ПАУ" },
            {"кинг", "КОНГ" },
            {"пинг", "ПОНГ" },
            {"за", "ВАРУДО" },
            {"дифоси", "ОТСОСИ" },
            {"пост стекла", "стекло вещь лосось" },
            {"Бот", "Я" }
        };

        public static Rarity RandomCard()
        {
            int val = rnd.Next(1000);
            if(val <= 535) { return Rarity.Mainstream; }
            else if(val > 535 && val <= 735) { return Rarity.Normal; }
            else if(val > 735 && val <= 875) { return Rarity.Rare; }
            else if (val > 875 && val <= 965) { return Rarity.Special; }
            else if(val > 965 && val <= 995) { return Rarity.Legendary; }
            else if(val > 995) { return Rarity.Mythycal; }
            else { return Rarity.Mainstream; }
        }

        public static Card GiveCard(long userID, Rarity rarity, int time)
        {
            var filtered = Cards.cardsList
                .Where(c => c.Rarity == rarity)
                .ToList();

            if (filtered.Count == 0) return null;

            var card = filtered[rnd2.Next(filtered.Count)];
            DataBase.AddCard(userID, card.ID, card.Rarity, time);

            return card;
        }
    }



}
