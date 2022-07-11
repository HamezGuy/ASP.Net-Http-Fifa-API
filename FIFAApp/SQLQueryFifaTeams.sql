create database FIFIAAPI

use FIFIAAPI

create table Teams(
	teamId int primary key identity(1000, 1),
	teamName varchar(100) unique,
	country varchar(100),
	teamFlag varchar(300),
	teamJersey varchar(300)
)

create table players
(
	playerTeamId int,
	playerId int primary key identity(0, 1),
	playerName varchar(100),
	playerPosition varchar(50), 
	playerNationality varchar(75),
	playerAge int,
	playerJerseyNumber int,
	playerWage float
	

	constraint fk_playerTeamId foreign key(playerTeamId) references Teams
)


