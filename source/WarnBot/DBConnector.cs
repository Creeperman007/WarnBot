using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace WarnBot
{
    class DBConnector
    {
        private static string host = "localhost";
        private static string username = "warnbotDB";
        private static string pass = "warnbot";
        private static string name = "warnbotdb";
        private static string cs = @"server=" + host + ";userid=" + username + ";password=" + pass + ";database=" + name;
        public static void Prepare(UInt64 user, UInt64 guild)
        {
            MySqlConnection conn = null;
            conn = new MySqlConnection(cs);
            conn.Open();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "SELECT * FROM `warnings` WHERE guild=" + guild + " AND user=@User";
            cmd.Prepare();
            cmd.Parameters.AddWithValue("@User", user);
            MySqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows.Equals(false))
            {
                reader.Close();
                cmd.Dispose();
                cmd.Connection = conn;
                cmd.CommandText = "INSERT INTO `warnings` (guild, user) VALUES (@Guild, @User)";
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@Guild", guild);
                cmd.ExecuteNonQuery();
            }
            conn.Close();

        }
        public static int WarnCount(UInt64 user, UInt64 guild)
        {
            MySqlConnection conn = null;
            conn = new MySqlConnection(cs);
            conn.Open();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "SELECT * FROM `warnings` WHERE guild=" + guild + " AND user=@User";
            cmd.Prepare();
            cmd.Parameters.AddWithValue("@User", user);
            MySqlDataReader reader = cmd.ExecuteReader();
            int warns = 0;
            while (reader.Read())
            {
                warns = Convert.ToInt32(reader["warnsCurrent"]);
            }
            conn.Close();
            return warns;
        }
        public static void Warn(UInt64 user, UInt64 guild, int warns)
        {
            MySqlConnection conn = null;
            conn = new MySqlConnection(cs);
            conn.Open();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "UPDATE warnings SET warnsCurrent=" + warns + " WHERE guild=" + guild + " AND user=@User";
            cmd.Prepare();
            cmd.Parameters.AddWithValue("@User", user);
            cmd.ExecuteNonQuery();
            cmd.Dispose();
            cmd.Connection = conn;
            cmd.CommandText = "UPDATE warnings SET warnsTotal=warnsTotal+1 WHERE guild=" + guild + " AND user=@User";
            cmd.Prepare();
            cmd.ExecuteNonQuery();
            if (conn != null)
            {
                conn.Close();
            }
        }
        public static void Kick(UInt64 user, UInt64 guild, string reason)
        {
            MySqlConnection conn = null;
            conn = new MySqlConnection(cs);
            conn.Open();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "UPDATE warnings SET kicks=kicks+1, kickReason=\"" + reason + "\" WHERE guild=" + guild + " AND user=@User";
            cmd.Prepare();
            cmd.Parameters.AddWithValue("@User", user);
            cmd.ExecuteNonQuery();
            if (conn != null)
            {
                conn.Close();
            }
        }
        public static void Ban(UInt64 user, UInt64 guild, string reason)
        {
            MySqlConnection conn = null;
            conn = new MySqlConnection(cs);
            conn.Open();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "UPDATE warnings SET banReason=\"" + reason + "\" WHERE guild=" + guild + " AND user=@User";
            cmd.Prepare();
            cmd.Parameters.AddWithValue("@User", user);
            cmd.ExecuteNonQuery();
            if (conn != null)
            {
                conn.Close();
            }
        }
        public static int[] Info(UInt64 user, UInt64 guild)
        {
            MySqlConnection conn = null;
            conn = new MySqlConnection(cs);
            conn.Open();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "SELECT * FROM `warnings` WHERE guild=" + guild + " AND user=@User";
            cmd.Prepare();
            cmd.Parameters.AddWithValue("@User", user);
            MySqlDataReader reader = cmd.ExecuteReader();
            int[] info = new int[3] { 0, 0, 0 };
            while (reader.Read())
            {
                info[0] = Convert.ToInt32(reader["warnsCurrent"]);
                info[1] = Convert.ToInt32(reader["kicks"]);
                info[2] = Convert.ToInt32(reader["warnsTotal"]);
            }
            conn.Close();
            return info;
        }
        public static void Clear(UInt64 user, UInt64 guild)
        {
            MySqlConnection conn = null;
            conn = new MySqlConnection(cs);
            conn.Open();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "UPDATE warnings SET warnsCurrent=0 WHERE guild=" + guild + " AND user=@User";
            cmd.Prepare();
            cmd.Parameters.AddWithValue("@User", user);
            cmd.ExecuteNonQuery();
            if (conn != null)
            {
                conn.Close();
            }
        }
        public static void AddUsr(UInt64 user, UInt64 guild, string perm)
        {
            bool kick;
            bool ban;
            if (perm.ToLower().Contains("kb"))
            {
                kick = true;
                ban = true;
            }
            else
            {
                kick = true;
                ban = false;
            }
            MySqlConnection conn = null;
            conn = new MySqlConnection(cs);
            conn.Open();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "INSERT INTO `permissions` (guild, user, kick, ban) VALUES (@Guild, @User, @Kick, @Ban)";
            cmd.Prepare();
            cmd.Parameters.AddWithValue("@User", user);
            cmd.Parameters.AddWithValue("@Guild", guild);
            cmd.Parameters.AddWithValue("@Kick", kick);
            cmd.Parameters.AddWithValue("@Ban", ban);
            cmd.ExecuteNonQuery();
            conn.Close();
        }
        public static void RmUsr(UInt64 user, UInt64 guild)
        {
            MySqlConnection conn = null;
            conn = new MySqlConnection(cs);
            conn.Open();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "DELETE FROM `permissions` WHERE user=@User AND guild=@Guild";
            cmd.Prepare();
            cmd.Parameters.AddWithValue("@User", user);
            cmd.Parameters.AddWithValue("@Guild", guild);
            cmd.ExecuteNonQuery();
            conn.Close();
        }
        public static void UpdateUsr(UInt64 user, UInt64 guild, string perm)
        {
            bool kick;
            bool ban;
            if (perm.ToLower().Contains("kb"))
            {
                kick = true;
                ban = true;
            }
            else
            {
                kick = true;
                ban = false;
            }
            MySqlConnection conn = null;
            conn = new MySqlConnection(cs);
            conn.Open();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "UPDATE `permissions` SET kick=@Kick, ban=@Ban WHERE guild=@Guild AND user=@User";
            cmd.Prepare();
            cmd.Parameters.AddWithValue("@User", user);
            cmd.Parameters.AddWithValue("@Guild", guild);
            cmd.Parameters.AddWithValue("@Kick", kick);
            cmd.Parameters.AddWithValue("@Ban", ban);
            cmd.ExecuteNonQuery();
            conn.Close();
        }
        public static Int16[] PermCheck(UInt64 user, UInt64 guild)
        {
            MySqlConnection conn = null;
            conn = new MySqlConnection(cs);
            conn.Open();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "SELECT * FROM `permissions` WHERE guild=" + guild + " AND user=@User";
            cmd.Prepare();
            cmd.Parameters.AddWithValue("@User", user);
            MySqlDataReader reader = cmd.ExecuteReader();
            Int16[] perms = new Int16[2];
            Int16 kick = 0;
            Int16 ban = 0;
            while (reader.Read())
            {
                kick += Convert.ToInt16(reader["kick"]);
                ban += Convert.ToInt16(reader["ban"]);
            }
            if (reader.VisibleFieldCount.Equals(0))
            {
                perms[0] = kick;
                perms[1] = ban;
                return perms;
            }
            conn.Close();
            perms[0] = kick;
            perms[1] = ban;
            return perms;
        }
    }
}
