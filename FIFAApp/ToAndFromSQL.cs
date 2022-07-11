    using System;
    using Microsoft.VisualBasic.FileIO;
    using System.Data.SqlClient;
    using System.Text.RegularExpressions;

namespace FIFAApp
{
    /// <summary>
    /// structure containing the table data
    /// </summary>
    [System.Serializable]
     public class TableStruct
    {
        public int teamId;
        public string? teamName;
        public string? country;
        public string? teamFlag;
        public string? teamJersey;
    }

    /// <summary>
    /// class containing the player data, serializable to json
    /// </summary>
    [System.Serializable]
    public class PlayerStruct
    {
        public int playerTeamId;
        public int playerId;
        public string? playerName;
        public string? playerPosition;
        public string? playerNationality;
        public int playerAge;
        public int playerJerseyNumber;
        public float playerWage;
        public string? playerProfile;
    }

    /// <summary>
    /// a datastructure that contains a talbe and the associated list of players and their info
    /// </summary>
    [System.Serializable]
    public class TableAndPlayerStruct
    {
        public TableStruct? table;
        public List<PlayerStruct>? players;
    }
    

    /// <summary>
    /// this class is responsible for all actions updating/retrieving from the sql database
    /// </summary>
    public class ToAndFromSQL
    {
        //fields
        private static readonly string fileLocationSQL = @"server = .\JAMES_INSTANCE; database = FIFIAAPI; integrated security = true"; //filelocation of sql

        /// <summary>
        /// deletes the player from the database
        /// </summary>
        /// <param name="playerName">player name to delete</param>
        /// <returns>false if unsuccessful, true if successful
        /// </returns>
        public bool DeletePlayer(string playerName)
        {
            SqlConnection connection = new SqlConnection(fileLocationSQL);

            SqlCommand sqlCommand = new SqlCommand("delete from players where playerName = @playerName", connection);

            sqlCommand.Parameters.AddWithValue("@playerName", System.Data.SqlDbType.VarChar).Value = playerName;


            try
            {
                connection.Open();

                sqlCommand.ExecuteNonQuery();

                connection.Close();

                return true;
            }
            catch (Exception)
            {
                connection.Close();
                return false;
            }

        }

        /// <summary>
        /// Updates only the team the player is on
        /// </summary>
        /// <param name="playerTeamId"> id to switch the player to (team to switch to)</param>
        /// <param name="playerId">player id, specific id to identify the player</param>
        /// <returns>false if unsuccesful, true if successful</returns>
        public bool UpdateSqlPlayer(int playerTeamId, int playerId)
        {
            SqlConnection connection = new SqlConnection(fileLocationSQL);

            SqlCommand sqlCommand = new SqlCommand("update players set playerTeamId = @playerTeamId where playerId = @playerId", connection);

            sqlCommand.Parameters.AddWithValue("@playerTeamId", System.Data.SqlDbType.Int).Value = playerTeamId;
            sqlCommand.Parameters.AddWithValue("@playerId", System.Data.SqlDbType.Int).Value = playerId;


            try
            {
                connection.Open();

                sqlCommand.ExecuteNonQuery();

                connection.Close();

                return true;
            }
            catch (Exception)
            {
                connection.Close();
                return false;
            }

        }

        /// <summary>
        /// Updates the information of the soccor team that is already in the database
        /// </summary>
        /// <param name="teamName">name of team</param>
        /// <param name="country">country of team</param>
        /// <param name="teamFlag">team flag link</param>
        /// <param name="teamJersey"> team jersey</param>
        /// <param name="teamId"> id of team</param>
        /// <returns>false if unsuccessful update, true if successful update</returns>
        public bool UpdateSqlTableTeams(string teamName, string country, string teamFlag, string teamJersey, int teamId)
        {
            SqlConnection connection = new SqlConnection(fileLocationSQL);

            SqlCommand sqlCommand = new SqlCommand("update Teams set teamName = @teamName, " +
                "country = @country, teamFlag = @teamFlag, teamJersey = @teamJersey where teamId = @teamId", connection);

            sqlCommand.Parameters.AddWithValue("@teamName", System.Data.SqlDbType.VarChar).Value = teamName;
            sqlCommand.Parameters.AddWithValue("@country", System.Data.SqlDbType.VarChar).Value = country;
            sqlCommand.Parameters.AddWithValue("@teamFlag", System.Data.SqlDbType.VarChar).Value = teamFlag;
            sqlCommand.Parameters.AddWithValue("@teamJersey", System.Data.SqlDbType.VarChar).Value = teamJersey;
            sqlCommand.Parameters.AddWithValue("@teamId", System.Data.SqlDbType.Int).Value = teamId;

            try
            {
                connection.Open();

                int success = sqlCommand.ExecuteNonQuery();
                if (success <= 0)
                {
                    throw new Exception();
                }

                connection.Close();

                return true;
            }
            catch (Exception)
            {
                connection.Close();
                return false;
            }

        }

        /// <summary>
        /// inserts player into sql database, uses team Id to identify their team
        /// </summary>
        /// <param name="playerTeamId"></param>
        /// <param name="playerId"></param>
        /// <param name="playerName"></param>
        /// <param name="playerPosition"></param>
        /// <param name="playerNationality"></param>
        /// <param name="playerAge"></param>
        /// <param name="playerJerseyNumber"></param>
        /// <param name="playerWage"></param>
        /// <param name="playerProfile"></param>
        /// <returns>false if unsuccessful insertion, true if successful</returns>
        public bool InsertIntoSqlPlayer(int playerTeamId, int playerId, string playerName, string playerPosition, string playerNationality, int playerAge, int playerJerseyNumber,
            float playerWage, string playerProfile)
        {
            SqlConnection connection = new SqlConnection(fileLocationSQL);
            //TODO i suppose i should check if it exists, should throw an error if it exists already anyhow
            SqlCommand sqlCommand = new SqlCommand("set identity_insert players on; " +
                "INSERT INTO players ([playerTeamId], [playerId], [playerName], " +
                "[playerPosition], [playerNationality], [playerAge], " +
                    "[playerJerseyNumber], [playerWage], [playerProfile]) VALUES(@playerTeamId, @playerId, @playerName, @playerPosition, " +
                    "@playerNationality, @playerAge, " +
                    "@playerJerseyNumber, @playerWage, @playerProfile); " +
                    "set identity_insert players off", connection);

            sqlCommand.Parameters.AddWithValue("@playerTeamId", System.Data.SqlDbType.Int).Value = playerTeamId;
            sqlCommand.Parameters.AddWithValue("@playerId", System.Data.SqlDbType.Int).Value = playerId;
            sqlCommand.Parameters.AddWithValue("@playerName", System.Data.SqlDbType.VarChar).Value = playerName;
            sqlCommand.Parameters.AddWithValue("@playerPosition", System.Data.SqlDbType.VarChar).Value = playerPosition;
            sqlCommand.Parameters.AddWithValue("@playerNationality", System.Data.SqlDbType.VarChar).Value = playerNationality;
            sqlCommand.Parameters.AddWithValue("@playerAge", System.Data.SqlDbType.Int).Value = playerAge;
            sqlCommand.Parameters.AddWithValue("@playerJerseyNumber", System.Data.SqlDbType.Int).Value = playerJerseyNumber;
            sqlCommand.Parameters.AddWithValue("@playerWage", System.Data.SqlDbType.Float).Value = playerWage;
            sqlCommand.Parameters.AddWithValue("@playerProfile", System.Data.SqlDbType.VarChar).Value = playerProfile;

            try
            {
                connection.Open();

                int success = sqlCommand.ExecuteNonQuery();
                if(success <= 0)
                {
                    throw new Exception();
                }

                connection.Close();

                return true;
            }
            catch (Exception)
            {
                connection.Close();
                return false;
            }
        }

        /// <summary>
        /// Inserts a new player into the sql database, overloaded method, inserts using teamName instead of team id
        /// </summary>
        /// parameters are self explanitory...
        /// <param name="teamName"></param>
        /// <param name="playerId"></param>
        /// <param name="playerName"></param>
        /// <param name="playerPosition"></param>
        /// <param name="playerNationality"></param>
        /// <param name="playerAge"></param>
        /// <param name="playerJerseyNumber"></param>
        /// <param name="playerWage"></param>
        /// <param name="playerProfile"></param>
        /// <returns>false if unseccessful insertion or error, true if player was successfully inserted</returns>
        /// <exception cref="Exception">exception occurs if there is an error inserting into the database, 
        /// or database doesn't exist, or some error executing</exception>
        public bool InsertIntoSqlPlayer(string teamName, int playerId, string playerName, string playerPosition, string playerNationality, int playerAge, int playerJerseyNumber,
            float playerWage, string playerProfile)
        {
            SqlConnection connection = new SqlConnection(fileLocationSQL);

            int? teamNumber = ReturnTeamNumber(teamName);
            if(teamNumber == null)
            {
                throw new Exception();
            }

            SqlCommand sqlCommand = new SqlCommand("set identity_insert players on; " +
                "INSERT INTO players ([playerTeamId], [playerId], [playerName], " +
                "[playerPosition], [playerNationality], [playerAge], " +
                    "[playerJerseyNumber], [playerWage], [playerProfile]) VALUES(@playerTeamId, @playerId, @playerName, @playerPosition, " +
                    "@playerNationality, @playerAge, " +
                    "@playerJerseyNumber, @playerWage, @playerProfile); " +
                    "set identity_insert players off", connection);

            sqlCommand.Parameters.AddWithValue("@playerTeamId", System.Data.SqlDbType.Int).Value = teamNumber;
            sqlCommand.Parameters.AddWithValue("@playerId", System.Data.SqlDbType.Int).Value = playerId;
            sqlCommand.Parameters.AddWithValue("@playerName", System.Data.SqlDbType.VarChar).Value = playerName;
            sqlCommand.Parameters.AddWithValue("@playerPosition", System.Data.SqlDbType.VarChar).Value = playerPosition;
            sqlCommand.Parameters.AddWithValue("@playerNationality", System.Data.SqlDbType.VarChar).Value = playerNationality;
            sqlCommand.Parameters.AddWithValue("@playerAge", System.Data.SqlDbType.Int).Value = playerAge;
            sqlCommand.Parameters.AddWithValue("@playerJerseyNumber", System.Data.SqlDbType.Int).Value = playerJerseyNumber;
            sqlCommand.Parameters.AddWithValue("@playerWage", System.Data.SqlDbType.Float).Value = playerWage;
            sqlCommand.Parameters.AddWithValue("@playerProfile", System.Data.SqlDbType.VarChar).Value = playerProfile;

            try
            {
                connection.Open();

                int success = sqlCommand.ExecuteNonQuery();
                if (success <= 0)
                {
                    throw new Exception();
                }

                connection.Close();

                return true;
            }
            catch (Exception)
            {
                connection.Close();
                return false;
            }
        }

        /// <summary>
        /// inserts a new team into sql database
        /// </summary>
        /// <param name="teamName"> team name </param>
        /// <param name="country"> team's country </param>
        /// <param name="teamFlag"> team's flag </param>
        /// <param name="teamJersey"> team's jersey</param>
        /// <returns>false if error, true if successful insertion</returns>
        public bool InsertIntoSqlTeam(string teamName, string country, string teamFlag, string teamJersey)
        {
            SqlConnection connection = new SqlConnection(fileLocationSQL);
            SqlCommand sqlCommand = new SqlCommand("INSERT INTO Teams ([teamName], [country], " +
                "[teamFlag], [teamJersey]) VALUES(@teamName, @country, @teamFlag, " +
                    "@teamJersey)", connection);

            sqlCommand.Parameters.AddWithValue("@teamName", System.Data.SqlDbType.VarChar).Value = teamName;
            sqlCommand.Parameters.AddWithValue("@country", System.Data.SqlDbType.VarChar).Value = country;
            sqlCommand.Parameters.AddWithValue("@teamFlag", System.Data.SqlDbType.VarChar).Value = teamFlag;
            sqlCommand.Parameters.AddWithValue("@teamJersey", System.Data.SqlDbType.VarChar).Value = teamJersey;

            try
            {
                connection.Open();

                int success = sqlCommand.ExecuteNonQuery();
                if(success <= 0)
                {
                    throw new Exception();
                }

                connection.Close();

                return true;
            }
            catch (Exception)
            {
                connection.Close();
                return false;
            }
        }

        /// <summary>
        /// gets the team id number of the team from sql, uses team name to find the teamNumber
        /// </summary>
        /// <param name="teamName">name of team searched</param>
        /// <returns>null if error, otherwise id of the team</returns>
        private int? ReturnTeamNumber(string teamName)
        {
            SqlConnection connection = new SqlConnection(fileLocationSQL);
            SqlCommand sqlCommand = new SqlCommand("select * from Teams where teamName = @teamName", connection);
            sqlCommand.Parameters.AddWithValue("@teamName", System.Data.SqlDbType.VarChar).Value = teamName;
            SqlDataReader sqlreader;
            int? returnNumber = null;

            try
            {
                connection.Open();

                sqlreader = sqlCommand.ExecuteReader();

                if (sqlreader.Read())
                {
                    returnNumber = Convert.ToInt32(sqlreader[0]);
                }

                connection.Close();

                return returnNumber;
            }
            catch (Exception)
            {
                connection.Close();
                return null;
            }

        }

        /// <summary>
        /// checks if player is in the sql database
        /// </summary>
        /// <param name="playerName">name of player being searched</param>
        /// <returns>true if player is present in database, false if not or error</returns>
        private bool CheckIfPlayerPresentSQL(string playerName)
        {
            SqlConnection connection = new SqlConnection(fileLocationSQL);
            SqlCommand sqlCommand = new SqlCommand("select count(*) from players where playerName = @playerName", connection);

            sqlCommand.Parameters.AddWithValue("@playerName", System.Data.SqlDbType.VarChar).Value = playerName;

            try
            {

                connection.Open();

                int returnNumber = Convert.ToInt32(sqlCommand.ExecuteScalar());

                if (returnNumber < 1)
                {
                    connection.Close();
                    return false;
                }

                connection.Close();
                return true;
            }
            catch (Exception)
            {
                connection.Close();
                return false;
            }

        }

        /// <summary>
        /// checks if team is in the database
        /// </summary>
        /// <param name="teamName"> name of team being searched </param>
        /// <returns> true if successful, team is present, false if team is not present or error </returns>
        private bool CheckIfTeamPresentSQL(string teamName)
        {
            SqlDataReader readRecord;

            try
            {
                SqlConnection connection = new SqlConnection(fileLocationSQL);
                SqlCommand sqlCommand = new SqlCommand("select count(*) from Teams where teamName = @teamName", connection);

                sqlCommand.Parameters.AddWithValue("@teamName", System.Data.SqlDbType.VarChar).Value = teamName;

                connection.Open();

                int returnNumber = Convert.ToInt32(sqlCommand.ExecuteScalar());

                if (returnNumber < 1)
                {
                    connection.Close();
                    return false;
                }

                connection.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }


        /// <summary>
        /// Shows all team names in database
        /// </summary>
        /// 
        /// <returns> 
        /// list of type tablestruct, returns a null if an error is encountered
        /// </returns>
        public List<TableStruct>? ShowAllTeams()
        {
            SqlConnection connection = new SqlConnection(fileLocationSQL);
            SqlCommand sqlCommand = new SqlCommand("select * from Teams", connection);
            SqlDataReader sqlreader;
            List<TableStruct>? returnTable = new List<TableStruct>();

            try
            {

                connection.Open();

                using (sqlreader = sqlCommand.ExecuteReader())
                {
                    while (sqlreader.Read())
                    {
                        TableStruct tempValue = new TableStruct();
                        tempValue.teamId = Convert.ToInt32(sqlreader[0]);
                        tempValue.teamName = Convert.ToString(sqlreader[1]);
                        tempValue.country = Convert.ToString(sqlreader[2]);
                        tempValue.teamFlag = Convert.ToString(sqlreader[3]);
                        tempValue.teamJersey = Convert.ToString(sqlreader[4]);

                        returnTable.Add(tempValue);
                    }

                }

                if(returnTable.Count <= 0)
                {
                    throw new Exception();
                }

                connection.Close();
                return returnTable;
            }
            catch (Exception)
            {
                connection.Close();
                return null;
            }

        }

        /// <summary>
        /// returns a team and all its player info
        /// </summary>
        /// 
        /// <param name="teamName">name of team</param>
        /// <returns>null if error, team and players if successful</returns>
        public TableAndPlayerStruct? ReturnTeamByName(string teamName)
        {
            try
            {
                TableAndPlayerStruct? returnTable = new TableAndPlayerStruct();
                returnTable.table = GetTeamByName(teamName);
                returnTable.players = GetTeamByNamePlayers(teamName);

                if(returnTable.players == null || returnTable.table == null)
                {
                    throw new Exception();
                }

                return returnTable;
            }
            catch(Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// (this will return, the whole team, team name and players)
        /// </summary>
        /// <param name="teamName"></param>
        /// <returns></returns>
        /// README yes i could have made a join table, but i already wrote this code so im going to stick with it
        private TableStruct? GetTeamByName(string teamName)
        {
            SqlConnection connection = new SqlConnection(fileLocationSQL);
            SqlCommand sqlCommand = new SqlCommand("select * from Teams where teamName = @teamName", connection);
            sqlCommand.Parameters.AddWithValue("@teamName", System.Data.SqlDbType.VarChar).Value = teamName;
            SqlDataReader sqlreader;
            TableStruct? returnTable = new TableStruct();

            try
            {

                connection.Open();

                using (sqlreader = sqlCommand.ExecuteReader())
                {
                    if (sqlreader.Read())
                    {
                        returnTable.teamId = Convert.ToInt32(sqlreader[0]);
                        returnTable.teamName = Convert.ToString(sqlreader[1]);
                        returnTable.country = Convert.ToString(sqlreader[2]);
                        returnTable.teamFlag = Convert.ToString(sqlreader[3]);
                        returnTable.teamJersey = Convert.ToString(sqlreader[4]);
                    }

                }

                connection.Close();
                return returnTable;
            }
            catch (Exception)
            {
                connection.Close();
                return null;
            }
        }

        /// <summary>
        /// players in a team and their details
        /// </summary>
        /// <param name="teamName">team name to search for</param>
        /// <returns>null if error, list of players and details if successful</returns>
        private List<PlayerStruct>? GetTeamByNamePlayers(string teamName)
        {
            TableStruct? tableValue = GetTeamByName(teamName);
            if (tableValue == null)
            {
                return null;
            }

            SqlConnection connection = new SqlConnection(fileLocationSQL);
            SqlCommand sqlCommand = new SqlCommand("select * from players where playerTeamId = @playerTeamId", connection);
            sqlCommand.Parameters.AddWithValue("@playerTeamId", System.Data.SqlDbType.Int).Value = tableValue.teamId;
            //README i guess i can check if int  = 0 to make sure teams aern't working
            SqlDataReader sqlreader;
            List<PlayerStruct>? returnList = new List<PlayerStruct>();


            try
            {
                connection.Open();

                using (sqlreader = sqlCommand.ExecuteReader())
                {
                    while (sqlreader.Read())
                    {
                        PlayerStruct? returnPlayer = new PlayerStruct();

                        returnPlayer.playerTeamId = Convert.ToInt32(sqlreader[0]);
                        returnPlayer.playerId = Convert.ToInt32(sqlreader[1]);
                        returnPlayer.playerName = Convert.ToString(sqlreader[2]);
                        returnPlayer.playerPosition = Convert.ToString(sqlreader[3]);
                        returnPlayer.playerNationality = Convert.ToString(sqlreader[4]);
                        returnPlayer.playerAge = Convert.ToInt32(sqlreader[5]);
                        returnPlayer.playerJerseyNumber = Convert.ToInt32(sqlreader[6]);
                        returnPlayer.playerWage = Convert.ToSingle(sqlreader[7]);
                        returnPlayer.playerProfile = Convert.ToString(sqlreader[8]);

                        returnList.Add(returnPlayer);
                    }

                }

                connection.Close();
                return returnList;
            }
            catch (Exception)
            {
                connection.Close();
                return null;
            }
        }

        /// <summary>
        /// Gets a player info by their name
        /// </summary>
        /// <param name="playerName"> the name of the player </param>
        /// <returns> null if error or nonexistant, player struct (w/player data) if successful </returns>
        public PlayerStruct? GetPlayerByName(string playerName)
        {
            SqlConnection connection = new SqlConnection(fileLocationSQL);
            SqlCommand sqlCommand = new SqlCommand("select * from players where playerName = @playerName", connection);
            sqlCommand.Parameters.AddWithValue("@playerName", System.Data.SqlDbType.VarChar).Value = playerName;
            SqlDataReader sqlreader;
            PlayerStruct? returnPlayer = new PlayerStruct();

            try
            {

                connection.Open();

                using (sqlreader = sqlCommand.ExecuteReader())
                {
                    if (sqlreader.Read())
                    {
                        returnPlayer.playerTeamId = Convert.ToInt32(sqlreader[0]);
                        returnPlayer.playerId = Convert.ToInt32(sqlreader[1]);
                        returnPlayer.playerName = Convert.ToString(sqlreader[2]);
                        returnPlayer.playerPosition = Convert.ToString(sqlreader[3]);
                        returnPlayer.playerNationality = Convert.ToString(sqlreader[4]);
                        returnPlayer.playerAge = Convert.ToInt32(sqlreader[5]);
                        returnPlayer.playerJerseyNumber = Convert.ToInt32(sqlreader[6]);
                        returnPlayer.playerWage = Convert.ToSingle(sqlreader[7]);
                        returnPlayer.playerProfile = Convert.ToString(sqlreader[8]);
                    }
                }

                connection.Close();

                if(returnPlayer.playerName == null)
                {
                    throw new Exception();
                }
                return returnPlayer;
            }
            catch (Exception)
            {
                connection.Close();
                return null;
            }
        }

        /// <summary>
        /// gets all the players by their position (midfield ect), uses abbrieviations
        /// </summary>
        /// 
        /// <param name="positionName">name of their position</param>
        /// <returns>null if error, else list of players (or none if none in their position)</returns>
        public List<PlayerStruct>? GetAllPlayersByPosition(string positionName)
        {
            SqlConnection connection = new SqlConnection(fileLocationSQL);
            SqlCommand sqlCommand = new SqlCommand("SELECT * FROM players WHERE playerPosition like @Position", connection);
            sqlCommand.Parameters.AddWithValue("@Position", System.Data.SqlDbType.VarChar).Value = "%" + positionName + "%";
            SqlDataReader sqlreader;
            List<PlayerStruct>? returnList = new List<PlayerStruct>();


            try
            {
                connection.Open();

                using (sqlreader = sqlCommand.ExecuteReader())
                {
                    while (sqlreader.Read())
                    {
                        PlayerStruct? returnPlayer = new PlayerStruct();

                        returnPlayer.playerTeamId = Convert.ToInt32(sqlreader[0]);
                        returnPlayer.playerId = Convert.ToInt32(sqlreader[1]);
                        returnPlayer.playerName = Convert.ToString(sqlreader[2]);
                        returnPlayer.playerPosition = Convert.ToString(sqlreader[3]);
                        returnPlayer.playerNationality = Convert.ToString(sqlreader[4]);
                        returnPlayer.playerAge = Convert.ToInt32(sqlreader[5]);
                        returnPlayer.playerJerseyNumber = Convert.ToInt32(sqlreader[6]);
                        returnPlayer.playerWage = Convert.ToSingle(sqlreader[7]);
                        returnPlayer.playerProfile = Convert.ToString(sqlreader[8]);

                        returnList.Add(returnPlayer);
                    }

                }

                connection.Close();
                return returnList;
            }
            catch (Exception)
            {
                connection.Close();
                return null;
            }
        }
    }

}
