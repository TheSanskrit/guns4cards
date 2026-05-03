using Microsoft.Data.Sqlite;
using Microsoft.VisualBasic;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;

namespace D4C
{


    public static class DataBase
    {
        static string dbPath = "MainBase2.db";
        static SqliteConnection connection = new SqliteConnection($"Data Source=/d4cdata/{dbPath}");


        public static void Connect()
        {
                connection.Open();
                Console.WriteLine("База пользователей создана или подключена.");

                string createTableQuery = "CREATE TABLE IF NOT EXISTS Users (Id INTEGER PRIMARY KEY, Nick TEXT, Score INTEGER, Coins INTEGER, LCT INTEGER, LCTBonus INTEGER)";
                using (var command = new SqliteCommand(createTableQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
            createTableQuery = "CREATE TABLE IF NOT EXISTS Cards (Id INTEGER, CardId INTEGER, CardAmount INTEGER, CardRarity TEXT, PRIMARY KEY(Id, CardId))";
            using (var command = new SqliteCommand(createTableQuery, connection))
            {
                command.ExecuteNonQuery();
            }
            Console.WriteLine("База карточек создана или подключена.");
            connection.Close();
        }



        public static void AddUser(long id, string name)
        {
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
        INSERT OR IGNORE INTO Users (Id, Nick, Score, Coins, LCT, LCTBonus) 
        VALUES ($id, $name, 0, 0, 0, 0);
    ";

            cmd.Parameters.AddWithValue("$id", id);
            cmd.Parameters.AddWithValue("$name", name);

            cmd.ExecuteNonQuery();
            connection.Close();
        }



        public static bool HasID(long id)
        {
            connection.Open();
            var command = connection.CreateCommand();

            command.CommandText = "SELECT COUNT(*) FROM Users WHERE Id = $id";
            command.Parameters.AddWithValue("$id", id);

            long count = (long)command.ExecuteScalar();
            connection.Close();

            return count > 0;
        }



        public static (string name, int score, int coins) GetStats(long id)
        {
            connection.Open();
            using (var command = new SqliteCommand("SELECT * FROM Users WHERE Id = $id", connection))
            {
                var cmd = connection.CreateCommand();

                cmd.CommandText = "SELECT Nick, Score, Coins FROM Users WHERE Id = $id";

                cmd.Parameters.AddWithValue("$id", id);

                string nick = "Ghost"; int score = 0; int coins = 0;

                using (var reader = cmd.ExecuteReader())
                {
                    if(reader.Read())
                    {
                        nick = reader.GetString(0);
                        score = reader.GetInt32(1);
                        coins = reader.GetInt32(2);
                    }
                }


                connection.Close();

                return (nick, score, coins);
            }
        }



        public static void ChangeName(string name, long id)
        {
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = @"UPDATE Users SET Nick = $name WHERE id = $id";

            cmd.Parameters.AddWithValue("$name", name);
            cmd.Parameters.AddWithValue("$id", id);

            cmd.ExecuteNonQuery();
            connection.Close();
        }
    


        public static void AddCard(long userID, int cardID, Rarity rarity, int unixTime, bool isBonus)
        {
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = @"INSERT INTO Cards (Id, CardId, CardAmount, CardRarity)
            VALUES ($user, $card, 1, $rarity)
            ON CONFLICT(Id, CardId)
            DO UPDATE SET CardAmount = CardAmount + 1;";

            cmd.Parameters.AddWithValue("$user", userID);
            cmd.Parameters.AddWithValue("$card", cardID);
            cmd.Parameters.AddWithValue("$rarity", rarity.ToString());

            cmd.ExecuteNonQuery();

            if (isBonus) { cmd.CommandText = @"UPDATE Users SET Score = Score + @score, Coins = Coins + @coins, LCTBonus = @time WHERE Id = @id"; }

            else { cmd.CommandText = @"UPDATE Users SET Score = Score + @score, Coins = Coins + @coins, LCT = @time WHERE Id = @id"; }

            cmd.Parameters.AddWithValue("@score", Cards.scoreRewards[(int)rarity]);
            cmd.Parameters.AddWithValue("@coins", Cards.coinsRewards[(int)rarity]);
            cmd.Parameters.AddWithValue("@id", userID);
            cmd.Parameters.AddWithValue("@time", unixTime);
            cmd.ExecuteNonQuery();

            connection.Close();
        }



        public static long GetCardsAmount(long userID) //returns amount of unique cards that user has
        {
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT COUNT(*) FROM Cards WHERE Id = @user";
            cmd.Parameters.AddWithValue("@user", userID);

            var result = cmd.ExecuteScalar();
            connection.Close();

            return result != DBNull.Value ? Convert.ToInt32(result) : 0;
        }



        public static (int mainstream, int normal, int rare, int special, int leg, int myth) GetRarityCards(long userID)
        {
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT 
    COUNT(CASE WHEN CardRarity = 'Mainstream' THEN 1 END),
    COUNT(CASE WHEN CardRarity = 'Normal' THEN 1 END),
    COUNT(CASE WHEN CardRarity = 'Rare' THEN 1 END),
    COUNT(CASE WHEN CardRarity = 'Special' THEN 1 END),
    COUNT(CASE WHEN CardRarity = 'Legendary' THEN 1 END),
    COUNT(CASE WHEN CardRarity = 'Mythical' THEN 1 END)
FROM Cards 
WHERE Id = @user;";

            cmd.Parameters.AddWithValue("@user", userID);

            var main = 0; var norm = 0; var rar = 0; var spec = 0; var legen = 0; var mythic = 0;

            using (var reader = cmd.ExecuteReader())
            {
                if(reader.Read())
                {
                    main = Convert.ToInt32(reader[0]);
                    norm = Convert.ToInt32(reader[1]);
                    rar = Convert.ToInt32(reader[2]);
                    spec = Convert.ToInt32(reader[3]);
                    legen = Convert.ToInt32(reader[4]);
                    mythic = Convert.ToInt32(reader[5]);
                }
            }
            connection.Close();

            return (main, norm, rar, spec, legen, mythic);        
        }



        public static (int[] score, int[] coins, string[] nick) Top()
        {
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                                SELECT Score, Coins, Nick FROM Users
                                ORDER BY Score DESC
                                LIMIT 10;";
            var scores = new List<int>();
            var nicks = new List<string>();
            var coins = new List<int>();

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    scores.Add(reader.GetInt32(0)); 
                    coins.Add(reader.GetInt32(1));
                    nicks.Add(reader.GetString(2));
                }
            }
            connection.Close();

            return (scores.ToArray(), coins.ToArray(), nicks.ToArray());
        }



        public static (bool, bool, int, int) Cooldown(long userID, int unixTime, int cooldown)
        {
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT LCT, LCTBonus FROM Users WHERE Id = @user";
            cmd.Parameters.AddWithValue("@user", userID);

            int LCT = 0; int LCTBonus = 0;

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    LCT = reader.GetInt32(0);
                    LCTBonus = reader.GetInt32(1);
                }
            }

            connection.Close();

            return (unixTime > Convert.ToInt64(LCT) + cooldown, unixTime > Convert.ToInt64(LCTBonus) + (3600 * 8), (Convert.ToInt32(LCT) + cooldown) - unixTime, (Convert.ToInt32(LCTBonus) + (3600 * 8)) - unixTime);
        }
    }
}
