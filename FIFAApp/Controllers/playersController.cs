using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections;


/*
 * This class is a controller
 * 
 * This class is responsible for HTTP requests
 * For players 
 * 
 * Written by James Gui, July 6, 2022
 */

namespace FIFAApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class playersController : Controller
    {
        //fields
        ToAndFromSQL sqlClass = new ToAndFromSQL(); //to and from sql class



        /// <summary>
        /// gets the player information from database based on player name
        /// </summary>
        /// <param name="playerName"> name of player searched </param>
        /// <returns>json data of player if successful, badresponse if failure</returns>
        [HttpGet]
        [Route("GetPlayerByName/{playerName}")]
        public IActionResult GetPlayerByName(string playerName)
        {
            try
            {
                PlayerStruct? returnPlayer = sqlClass.GetPlayerByName(playerName);
                if (returnPlayer == null)
                {
                    throw new Exception("Error retrieving player");
                }

                var returnItem = new
                {
                    playerTeamId = returnPlayer.playerTeamId,
                    playerId = returnPlayer.playerId,
                    playerName = returnPlayer.playerName,
                    playerPosition = returnPlayer.playerPosition,
                    playerNationality = returnPlayer.playerNationality,
                    playerAge = returnPlayer.playerAge,
                    playerJerseyNumber = returnPlayer.playerJerseyNumber,
                    playerWage = returnPlayer.playerWage,
                    playerProfile = returnPlayer.playerProfile,

                };

                return Ok(returnItem);
            }
            catch (Exception es)
            {
                return BadRequest(es.Message);
            }
        }

        /// <summary>
        /// gets all players that play a certain soccor position
        /// </summary>
        /// <param name="positionName">soccer position name</param>
        /// <returns>list of json objects (players that play position and their info) if successful
        /// badrequest if there is a failure</returns>
        [HttpGet]
        [Route("GetAllPlayerByPosition/{positionName}")]
        public IActionResult GetPlayerByPosition(string positionName)
        {
            ArrayList returnArray = new ArrayList();
            try
            {
                List<PlayerStruct>? returnPlayer = sqlClass.GetAllPlayersByPosition(positionName);
                if (returnPlayer == null)
                {
                    throw new Exception("Error retrieving player");
                }

                foreach (PlayerStruct player in returnPlayer)
                {
                    var returnItem = new
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
                    returnArray.Add(returnItem);
                }

                return Ok(returnArray);
            }
            catch (Exception es)
            {
                return BadRequest(es.Message);
            }
        }

        /// <summary>
        /// adds a new player into the database based on teamName
        /// </summary>
        /// <param name="teamName"></param>
        /// <param name="playerId"></param>
        /// <param name="playerName"></param>
        /// <param name="playerPosition"></param>
        /// <param name="playerNationality"></param>
        /// <param name="playerAge"></param>
        /// <param name="playerJerseyNumber"></param>
        /// <param name="playerWage"></param>
        /// <param name="playerProfile"></param>
        /// <returns>ok upon success, badrequest upon failure</returns>
        [HttpPost]
        [Route("AddNewPlayerWithTeamName/{playerTeamId},{playerId},{playerName}, {playerPosition}, {playerNationality}, " +
            "{playerAge}, {playerJerseyNumber}, {playerWage}, {playerProfile}")]
        public IActionResult AddNewPlayerWithTeamName(string teamName, int playerId, string playerName, string playerPosition,
            string playerNationality, int playerAge, int playerJerseyNumber,
            float playerWage, string playerProfile)
        {
            try
            {
                if (!sqlClass.InsertIntoSqlPlayer(teamName, playerId, playerName, playerPosition,
                    playerNationality, playerAge, playerJerseyNumber, playerWage, playerProfile))
                {
                    throw new Exception("issue inserting into player");
                }
                return Ok("successfully entered new player");
            }
            catch (Exception es)
            {
                return BadRequest(es.Message);
            }
        }

        /// <summary>
        /// adds a new player into the database based on teamId
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
        /// <returns>ok upon success, badrequest upon failure</returns>
        [HttpPost]
        [Route("AddNewPlayer/{playerTeamId},{playerId},{playerName}, {playerPosition}, {playerNationality}, " +
            "{playerAge}, {playerJerseyNumber}, {playerWage}, {playerProfile}")]
        public IActionResult AddNewPlayer(int playerTeamId, int playerId, string playerName, string playerPosition,
            string playerNationality, int playerAge, int playerJerseyNumber,
            float playerWage, string playerProfile)
        {
            try
            {
                if (!sqlClass.InsertIntoSqlPlayer(playerTeamId, playerId, playerName, playerPosition,
                    playerNationality, playerAge, playerJerseyNumber, playerWage, playerProfile))
                {
                    throw new Exception("issue inserting into player");
                }
                return Ok("successfully entered new player");
            }
            catch (Exception es)
            {
                return BadRequest(es.Message);
            }
        }

        /// <summary>
        /// updates the team the player plays for
        /// </summary>
        /// <param name="playerTeamId">player specific id</param>
        /// <param name="playerId"> the team the player plays for, supposed to be a new team</param>
        /// <returns>ok upon success, badrequst upon failure</returns>
        [HttpPut]
        [Route("UpdatePlayer/{playerTeamId}, {playerId}")]
        public IActionResult UpdatePlayer(int playerTeamId, int playerId)
        {
            try
            {
                if (!sqlClass.UpdateSqlPlayer(playerTeamId, playerId))
                {
                    throw new Exception("Issue Updating Account");
                }

                return Ok("Account Successfully Updated");
            }
            catch (Exception es)
            {
                return BadRequest(es.Message);
            }
        }

        /// <summary>
        /// deletes a player from the database
        /// </summary>
        /// <param name="playerName">player name to be deleted</param>
        /// <returns>ok upon success, badrequst if failure</returns>
        [HttpDelete]
        [Route("DeletePlayer/{playerName}")]
        public IActionResult DeletePlayer(string playerName)
        {
            try
            {
                if (!sqlClass.DeletePlayer(playerName))
                {
                    throw new Exception("Issue deleting player");
                }
                return Ok("successfullly deleted player");
            }
            catch (Exception es)
            {
                return BadRequest(es.Message);
            }
        }
    }
}
