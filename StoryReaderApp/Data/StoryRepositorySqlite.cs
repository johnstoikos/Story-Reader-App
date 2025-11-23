using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using StoryReaderApp.Models;

namespace StoryReaderApp.Data
{
    public class StoryRepositorySqlite : IStoryRepository
    {
        public IDbConnection OpenConnection()
        {
            var c = DbFactory.Create();
            c.Open();
            return c;
        }

        public IEnumerable<string> GetCategories()
        {
            var list = new List<string>();
            try
            {
                using (IDbConnection conn = OpenConnection())
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT DISTINCT Category FROM Stories ORDER BY Category;";
                    using (IDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                            list.Add(r.GetString(0));
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Αποτυχία ανάγνωσης κατηγοριών από τη βάση.", ex);
            }
            return list;
        }

        public IEnumerable<Story> GetAll()
        {
            var list = new List<Story>();
            try
            {
                using (IDbConnection conn = OpenConnection())
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT ID,Title,Category,Content FROM Stories ORDER BY Title;";
                    using (IDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            list.Add(new Story
                            {
                                Id = r.GetInt32(0),
                                Title = r.GetString(1),
                                Category = r.GetString(2),
                                Content = r.GetString(3)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Αποτυχία ανάγνωσης ιστοριών από τη βάση.", ex);
            }
            return list;
        }

        public IEnumerable<Story> GetByCategory(string category)
        {
            var list = new List<Story>();
            try
            {
                using (IDbConnection conn = OpenConnection())
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT ID,Title,Category,Content FROM Stories WHERE Category=@cat ORDER BY Title;";
                    cmd.Parameters.Add(new SQLiteParameter("@cat", category ?? string.Empty));

                    using (IDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            list.Add(new Story
                            {
                                Id = r.GetInt32(0),
                                Title = r.GetString(1),
                                Category = r.GetString(2),
                                Content = r.GetString(3)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Αποτυχία ανάγνωσης ιστοριών ανά κατηγορία από τη βάση.", ex);
            }
            return list;
        }

        public int Create(Story s)
        {
            if (s == null) throw new ArgumentNullException(nameof(s));
            try
            {
                using (IDbConnection conn = OpenConnection())
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText =
                        "INSERT INTO Stories(Title,Category,Content) " +
                        "VALUES(@t,@c,@x); SELECT last_insert_rowid();";

                    cmd.Parameters.Add(new SQLiteParameter("@t", s.Title ?? string.Empty));
                    cmd.Parameters.Add(new SQLiteParameter("@c", s.Category ?? string.Empty));
                    cmd.Parameters.Add(new SQLiteParameter("@x", s.Content ?? string.Empty));

                    object result = cmd.ExecuteScalar();
                    long id64 = (result is long) ? (long)result : 0L;
                    return (int)id64;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Αποτυχία δημιουργίας ιστορίας.", ex);
            }
        }

        public void Update(Story s)
        {
            if (s == null) throw new ArgumentNullException(nameof(s));
            try
            {
                using (IDbConnection conn = OpenConnection())
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText =
                        "UPDATE Stories SET Title=@t, Category=@c, Content=@x WHERE ID=@id;";
                    cmd.Parameters.Add(new SQLiteParameter("@t", s.Title ?? string.Empty));
                    cmd.Parameters.Add(new SQLiteParameter("@c", s.Category ?? string.Empty));
                    cmd.Parameters.Add(new SQLiteParameter("@x", s.Content ?? string.Empty));
                    cmd.Parameters.Add(new SQLiteParameter("@id", s.Id));

                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Αποτυχία ενημέρωσης ιστορίας.", ex);
            }
        }

        public void Delete(int id)
        {
            try
            {
                using (IDbConnection conn = OpenConnection())
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM Stories WHERE ID=@id;";
                    cmd.Parameters.Add(new SQLiteParameter("@id", id));
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Αποτυχία διαγραφής ιστορίας.", ex);
            }
        }
    }
}
