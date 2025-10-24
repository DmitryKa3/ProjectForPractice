using System.Data.SQLite;

namespace Практика2курс
{
    class DataBase
    {
        private SQLiteConnection sqliteConnection = new SQLiteConnection("Data Source=bdprakt2k.db;Version=3;");

        public void openConnection()
        {
            if (sqliteConnection.State == System.Data.ConnectionState.Closed)
                sqliteConnection.Open();
        }

        public void closeConnection()
        {
            if (sqliteConnection.State == System.Data.ConnectionState.Open)
                sqliteConnection.Close();
        }

        public SQLiteConnection getConnection()
        {
            return sqliteConnection;
        }
    }
}