using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections;

/*
 * This class is a controller
 * 
 * This class is responsible for HTTP requests
 * For teams 
 * 
 * Written by James Gui, July 6, 2022
 */

namespace FIFAApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FifaHttpController : Controller
    {
        //fields
        ToAndFromSQL sqlClass = new ToAndFromSQL(); //to and from sql class

        /// <summary>
        /// gets all the team names from the database
        /// </summary>
        /// <returns>bad result if error, json array if successful</returns>
        [HttpGet]
        [Route("AllTeams")]
        public IActionResult GetAllTeams()
        {
            try
            {
                List<TableStruct>? allTeams = sqlClass.ShowAllTeams();
                if(allTeams == null)
                {
                    throw new Exception("Error retrieving all the team info or no teams present");
                }

                ArrayList array = new ArrayList();

                foreach(TableStruct team in allTeams)
                {
                    var testItem = new { 
                    teamId = team.teamId,
                    teamName = team.teamName,
                    country = team.country,
                    teamFlag = team.teamFlag,
                    teamJersey = team.teamJersey
                    };
                    array.Add(testItem);
                } 

                return Ok(array);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// gets specific team and their players 
        /// </summary>
        /// <param name="teamName">name of team searched</param>
        /// <returns>json object if successful, badresponse if failure</returns>
        [HttpGet]
        [Route("GetTeamByName/{teamName}")]
        public IActionResult GetTeamByName(string teamName)
        {
            ArrayList playerArray = new ArrayList();

            try
            {
                TableAndPlayerStruct? returnTable = sqlClass.ReturnTeamByName(teamName);
                if(returnTable == null)
                {
                    throw new Exception("Error retrieving all the teams or no teams present");
                }

                if(returnTable.table == null || returnTable.players == null)
                {
                    throw new Exception("Error retrieving all the teams or no teams present");
                }

                var returnTeam = new
                {
                    teamId = returnTable.table.teamId,
                    teamName = returnTable.table.teamName,
                    country = returnTable.table.country,
                    teamFlag = returnTable.table.teamFlag,
                    teamJersey = returnTable.table.teamJersey
                };
                playerArray.Add(returnTeam);


                foreach (PlayerStruct player in returnTable.players)
                {
                    var playerObject = new
                    {
                        playerTeamId = player.playerTeamId,
                        playerId = player.playerId,
                        playerName = player.playerName,
                        playerPosition = player.playerPosition,
                        playerNationality = player.playerNationality,
                        playerAge = player.playerAge,
                        playerJerseyNumber = player.playerJerseyNumber,
                        playerWage = player.playerWage,
                        playerProfile = player.playerProfile,
                      };
                    playerArray.Add(playerObject);
                }

                

                return Ok(playerArray);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// adds a team to the database, from user ui (swagger atm)
        /// </summary>
        /// self explanitory params
        /// <param name="teamName"></param>
        /// <param name="country"></param>
        /// <param name="teamFlag"></param>
        /// <param name="teamJersey"></param>
        /// <returns>ok if true, badrequest if an issue occurs</returns>
        [HttpPost]
        [Route("AddNewTeam/{teamName}, {country}, {teamFlag}, {teamJersey}")]
        public IActionResult AddNewTeam(string teamName, string country, string teamFlag, string teamJersey)
        {
            try
            {
                bool successBool = sqlClass.InsertIntoSqlTeam(teamName, country, teamFlag, teamJersey);
                if(!successBool)
                {
                    throw new Exception("Error inserting team into database");
                }
                return Ok("Successful Insertion");
            }
            catch (Exception es)
            {
                return BadRequest(es.Message);
            }
        }

        /// <summary>
        /// updates the values in the team, finds the team based on the id
        /// </summary>
        /// <param name="teamId"></param>
        /// <param name="teamName"></param>
        /// <param name="country"></param>
        /// <param name="teamFlag"></param>
        /// <param name="teamJersey"></param>
        /// <returns>ok if successful, badrequest if an error occurs</returns>
        [HttpPut]
        [Route("UpdateTeam/{teamId} , {teamName}, {country}, {teamFlag}, {teamJersey}")]
        public IActionResult UpdateTeam(int teamId, string teamName, string country, string teamFlag, string teamJersey)
        {
            try
            {
                bool successBool = sqlClass.UpdateSqlTableTeams(teamName, country, teamFlag, teamJersey, teamId);
                if (!successBool)
                {
                    throw new Exception("Error inserting team into database");
                }
                return Ok("Successful Insertion");
            }
            catch (Exception es)
            {
                return BadRequest(es.Message);
            }
        }

        

    }
}
