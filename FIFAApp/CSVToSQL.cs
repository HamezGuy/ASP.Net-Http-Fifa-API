
    using System;
    using Microsoft.VisualBasic.FileIO;
    using System.Data.SqlClient;
    using System.Text.RegularExpressions;

    //will not document this b/c its not part of main project requirement, parsing data

        namespace InsertIntoTableSQL
        {
            public class CSVToSQL
            {
                string?[][] rows = new string[100000][];
                private string csvfilepath = @"players_21.csv";
                //TODO must change server for this to work
                private static readonly string fileLocationSQL = @"server = .\JAMES_INSTANCE; database = FIFIAAPI; integrated security = true";

                public CSVToSQL()
                {

                    ParseFromCSV();
                    InsertIntoTeamsTableSQL();
                    InsertIntoPlayersTableSQL();
                    UpdateAll();
                }

                public bool InsertIntoTeamsTableSQL()
                {
                    try
                    {
                        if (rows == null)
                        {
                            throw new Exception();
                        }

                        for (int i = 1; i < rows.GetLength(0); i++)
                        {

                            if (rows[i] == null)
                            {
                                continue;
                            }

                            if (rows[i][10] == null || rows[i][9] == null)
                            {
                                Console.WriteLine("null value table insert");
                                continue;
                            }

                            rows[i][9] = Convert.ToString(rows[i][9]);
                            rows[i][10] = Convert.ToString(rows[i][10]);

                            //WILL HAVE TO PUT IN TEAMS EVEN IF THEY'RE WIERD Names
                            if (!RegexMatching(RegexReplaceSpaces(rows[i][9])) || !RegexMatching(RegexReplaceSpaces(rows[i][10])))
                            {
                                continue;
                            }

                            if (!CheckIfTeamPresentSQL(rows[i][9]))
                            {
                                if (!InsertIntoSqlTeam(rows[i][9], rows[i][10]))
                                {
                                    Console.WriteLine("issue inserting into table");
                                }
                            }

                        }
                    }
                    catch (Exception)
                    {
                        return false;
                    }

                    return true;
                }

                public bool InsertIntoPlayersTableSQL()
                {
                    try
                    {
                        if (rows == null)
                        {
                            throw new Exception();
                        }

                        for (int i = 1; i < rows.GetLength(0); i++)
                        {
                            if (rows[i] == null)
                            {
                                continue;
                            }

                            if (rows[i][3] == null || rows[i][0] == null || rows[i][26] == null ||
                            rows[i][8] == null || rows[i][4] == null || rows[i][27] == null || rows[i][14] == null
                            || rows[i][10] == null || rows[i][9] == null || rows[i][1] == null)
                            {
                                continue;
                            }


                            if (!RegexMatching(RegexReplaceSpaces(rows[i][3])) || !RegexMatching(RegexReplaceSpaces(rows[i][8])))
                            {
                                continue;
                            }

                            if (!RegexMatching(RegexReplaceSpaces(rows[i][9])) || !RegexMatching(RegexReplaceSpaces(rows[i][10])))
                            {
                                continue;
                            }

                            if (!CheckIfPlayerPresentSQL(rows[i][3]))
                            {
                                int? tableNumber = ReturnTeamNumber(rows[i][9]);
                                if (!tableNumber.HasValue)
                                {
                                    continue;
                                }

                                int playerId = Convert.ToInt32(rows[i][0]);
                                int playerAge = Convert.ToInt32(rows[i][4]);
                                int playerJerseyNumber = Convert.ToInt32(rows[i][27]);
                                float playerWage = Convert.ToInt32(rows[i][14]);

                                if (!InsertIntoSqlPlayer(tableNumber.Value, playerId, rows[i][3], rows[i][26],
                                    rows[i][8], playerAge, playerJerseyNumber, playerWage, rows[i][1]))
                                {
                                    Console.WriteLine("issue inserting into player table");
                                }
                            }

                        }

                    }
                    catch (Exception)
                    {
                        return false;
                    }

                    return true;
                }

                private bool RegexMatching(string str)
                {
                    if (Regex.IsMatch(str, @"^[a-zA-Z0-9]+$"))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                private string RegexReplaceSpaces(string str)
                {
                    return Regex.Replace(str, @"\s+", "");
                }

                private bool InsertIntoSqlPlayer(int playerTeamId, int playerId, string playerName, string playerPosition, string playerNationality, int playerAge, int playerJerseyNumber,
                    float playerWage, string playerProfile)
                {
                    SqlConnection connection = new SqlConnection(fileLocationSQL);

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

                private bool InsertIntoSqlTeam(string teamName, string country)
                {
                    SqlConnection connection = new SqlConnection(fileLocationSQL);
                    SqlCommand sqlCommand = new SqlCommand("INSERT INTO Teams ([teamName], [country], " +
                        "[teamFlag], [teamJersey]) VALUES(@teamName, @country, @teamFlag, " +
                            "@teamJersey)", connection);

                    sqlCommand.Parameters.AddWithValue("@teamName", System.Data.SqlDbType.VarChar).Value = teamName;
                    sqlCommand.Parameters.AddWithValue("@country", System.Data.SqlDbType.VarChar).Value = country;
                    sqlCommand.Parameters.AddWithValue("@teamFlag", System.Data.SqlDbType.VarChar).Value = "";
                    sqlCommand.Parameters.AddWithValue("@teamJersey", System.Data.SqlDbType.VarChar).Value = "";

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

                private bool UpdateAllEnterSql(string playerName, string playerPosition)
                {
                    SqlConnection connection = new SqlConnection(fileLocationSQL);
                    SqlCommand sqlCommand = new SqlCommand("update players set playerPosition = @playerPosition where playerName = @playerName", connection);

                    sqlCommand.Parameters.AddWithValue("@playerPosition", System.Data.SqlDbType.VarChar).Value = playerPosition;
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

                private bool UpdateAll()
                {
                    try
                    {
                        if (rows == null)
                        {
                            throw new Exception();
                        }

                        for (int i = 1; i < rows.GetLength(0); i++)
                        {
                            if (rows[i] == null)
                            {
                                continue;
                            }

                            if (rows[i][3] == null || rows[i][0] == null || rows[i][26] == null ||
                            rows[i][8] == null || rows[i][4] == null || rows[i][27] == null || rows[i][14] == null
                            || rows[i][10] == null || rows[i][9] == null || rows[i][1] == null)
                            {
                                continue;
                            }
                            //i 16

                            if (!RegexMatching(RegexReplaceSpaces(rows[i][3])) || !RegexMatching(RegexReplaceSpaces(rows[i][8])))
                            {
                                continue;
                            }

                            if (!RegexMatching(RegexReplaceSpaces(rows[i][9])) || !RegexMatching(RegexReplaceSpaces(rows[i][10])))
                            {
                                continue;
                            }

                            if (CheckIfPlayerPresentSQL(rows[i][3]))
                            {

                                if (!UpdateAllEnterSql(rows[i][3], rows[i][16]))
                                {
                                    Console.WriteLine("issue inserting into player table");
                                }
                            }

                        }

                    }
                    catch (Exception)
                    {
                        return false;
                    }

                    return true;
                }

                public bool ParseFromCSV()
                {
                    try
                    {
                        using (TextFieldParser textFieldParser = new TextFieldParser(csvfilepath))
                        {
                            if (textFieldParser == null)
                            {
                                Console.WriteLine("no value found for file");
                                throw new NullReferenceException();
                            }

                            textFieldParser.TextFieldType = FieldType.Delimited;
                            textFieldParser.SetDelimiters(",");

                            int rowNumber = 0;
                            while (!textFieldParser.EndOfData && rowNumber < 20000)
                            {
                                rows[rowNumber] = textFieldParser.ReadFields();

                                if (rows[rowNumber] == null)
                                {
                                    return true;
                                }

                                if (rows[rowNumber] != null)
                                {
                                    Console.WriteLine("this is value " + rows[rowNumber][1]);
                                    rowNumber++;
                                }

                            }
                        }
                    }
                    catch (Exception)
                    {
                        return false;
                    }

                    return true;
                }
            }
        }

    


