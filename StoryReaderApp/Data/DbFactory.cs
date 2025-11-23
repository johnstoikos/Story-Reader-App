using System.Data;
using System.Data.SQLite;
using System.IO;

namespace StoryReaderApp.Data
{
    public static class DbFactory
    {
        public static string DbPath { get { return Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Stories.sqlite"); } }
        public static string ConnStr { get { return "Data Source=" + DbPath + ";Version=3;"; } }
        public static bool Exists() { return File.Exists(DbPath); }
        public static IDbConnection Create() { return new SQLiteConnection(ConnStr); }

        public static void EnsureCreated()
        {
            if (Exists()) return;
            SQLiteConnection.CreateFile(DbPath);
            using (IDbConnection conn = Create())
            {
                conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText =
                        "CREATE TABLE Stories(" +
                        " ID INTEGER PRIMARY KEY AUTOINCREMENT," +
                        " Title TEXT NOT NULL," +
                        " Category TEXT NOT NULL," +
                        " Content TEXT NOT NULL" +
                        ");";
                    cmd.ExecuteNonQuery();
                }
                using (IDbTransaction tx = conn.BeginTransaction())
                using (IDbCommand insert = conn.CreateCommand())
                {
                    insert.Transaction = tx;
                    insert.CommandText = "INSERT INTO Stories(Title,Category,Content) VALUES (@t,@c,@x);";
                    var pT = new SQLiteParameter("@t");
                    var pC = new SQLiteParameter("@c");
                    var pX = new SQLiteParameter("@x");
                    insert.Parameters.Add(pT); insert.Parameters.Add(pC); insert.Parameters.Add(pX);

                    Seed(insert, pT, pC, pX,
                        "Cinderella",
                        "Fairy Tale",
                        "Once upon a time, in a faraway kingdom, there lived a kind girl named Cinderella. She was forced to serve her wicked stepmother and stepsisters, but she always remained gentle and good-hearted. One day, a royal ball was announced, and with the help of her fairy godmother, Cinderella attended in a magical gown. The prince fell in love with her, but at midnight she had to flee, leaving behind a glass slipper. The prince searched the kingdom until he found her, and they lived happily ever after."
                    );

                    Seed(insert, pT, pC, pX,
                        "Little Red Riding Hood",
                        "Fairy Tale",
                        "There once was a little girl who always wore a red hood. One day, she went to visit her grandmother who lived deep in the forest. On her way, she met a cunning wolf who rushed to the grandmother’s house and disguised himself. When Little Red Riding Hood arrived, she noticed her grandmother looked strange. The wolf jumped out to eat her, but a woodcutter heard the cries and saved them. The wolf was chased away, and Little Red Riding Hood learned to never talk to strangers."
                    );

                    Seed(insert, pT, pC, pX,
                        "Pinocchio",
                        "Fantasy",
                        "A poor woodcarver named Geppetto created a wooden puppet called Pinocchio. One night, a fairy brought Pinocchio to life. Pinocchio was curious and adventurous, but also mischievous and often told lies. Each time he lied, his nose grew longer. After many adventures, Pinocchio learned the value of honesty and courage, and the fairy rewarded him by turning him into a real boy."
                    );

                    Seed(insert, pT, pC, pX,
                        "Alice in Wonderland",
                        "Fantasy",
                        "Alice was a curious young girl who followed a white rabbit into a rabbit hole. She fell into a magical land full of strange creatures, like the Cheshire Cat, the Mad Hatter, and the Queen of Hearts. She joined a peculiar tea party, played a bizarre game of croquet, and faced many challenges. In the end, Alice discovered it was all a dream, but the adventure remained in her heart forever."
                    );

                    Seed(insert, pT, pC, pX,
                        "Peter Pan",
                        "Adventure",
                        "Peter Pan, the boy who never grew up, lived in Neverland with the Lost Boys. One night, he took Wendy and her brothers on a magical flight to his island. They fought pirates led by the evil Captain Hook, met fairies like Tinker Bell, and had many adventures. In the end, Wendy and her brothers chose to return home, but Peter remained in Neverland, forever young and free."
                    );

                    Seed(insert, pT, pC, pX,
                        "Snow White and the Seven Dwarfs", "Classics",
                        "Snow White was a princess whose beauty was envied by her stepmother, the Queen. Forced to flee, she found shelter with seven dwarfs...");

                    Seed(insert, pT, pC, pX,
                        "Hansel and Gretel", "Classics",
                        "Hansel and Gretel, lost in the forest, discovered a house made of sweets. But the witch inside had dark plans...");

                    Seed(insert, pT, pC, pX,
                        "Jack and the Beanstalk", "Adventure",
                        "Jack traded his cow for magic beans. Overnight, they grew into a giant beanstalk that reached the sky...");

                    Seed(insert, pT, pC, pX,
                        "Rapunzel", "Fairy Tales",
                        "Rapunzel was a girl with long golden hair, locked in a tower by a witch. Her song led a prince to her rescue...");

                    Seed(insert, pT, pC, pX,
                        "The Ugly Duckling", "Classics",
                        "A small duckling was mocked for being different, until he grew into a beautiful swan admired by all...");

                    Seed(insert, pT, pC, pX,
                        "Beauty and the Beast", "Romance",
                        "Belle, a kind and intelligent girl, saw beyond the Beast's frightening appearance and discovered his true heart...");

                    Seed(insert, pT, pC, pX,
                        "Aladdin and the Magic Lamp", "Adventure",
                        "Aladdin discovered a magical lamp with a powerful genie inside, who granted him three wishes...");

                    Seed(insert, pT, pC, pX,
                        "The Little Mermaid", "Fantasy",
                        "A young mermaid dreamed of living on land and fell in love with a human prince, but the sea witch demanded a great sacrifice...");

                    tx.Commit();
                }
            }
        }

        private static void Seed(IDbCommand insert, SQLiteParameter pT, SQLiteParameter pC, SQLiteParameter pX, string t, string c, string x)
        {
            pT.Value = t; pC.Value = c; pX.Value = x; insert.ExecuteNonQuery();
        }
    }
}
