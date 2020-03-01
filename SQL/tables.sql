create table Users (
	Usr_UsrId int auto_increment primary key,
    Usr_Email varchar(300),
    Usr_Hash varchar(9999),
    Usr_Name varchar(100),
    Usr_Surname varchar(100)
);

create table Users_Active (
	UAc_UAcId int auto_increment primary key,
    UAc_UsrId int,
    UAc_Active bool default false,
    UAc_ActivationKey varchar(10)
);

create table `Groups` (
	Grp_GrpId int auto_increment primary key,
    Grp_Name varchar(100)
);

create table Groups_Join (
	GJo_GJoId int auto_increment primary key,
    GJo_GrpId int,
	GJo_JoiningKey varchar(20),
    GJo_ExpirationDate datetime
);

create table Groups_Members (
	GMe_GmeId int auto_increment primary key,
    GMe_GrpId int,
    GMe_UsrId int,
    GMe_Role varchar(20)
);

create table Channels  (
	Cha_ChaId int auto_increment primary key,
    Cha_GrpId int,
    Cha_ConId int null,
    Cha_Name varchar(300)
);

create table Conferences(
	Con_ConId int auto_increment primary key,
    Con_GrpId int,
    Con_Topic varchar(300),
    Con_Description varchar(3000),
    Con_StartDate datetime,
    Con_State int
);

create table Conferences_Results(
	CRs_CRsId int auto_increment primary key,
    CRs_ConId int,
    CRs_ConState int,
    CRS_UsrId int,
    CRs_UsrResult double
);