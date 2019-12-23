delimiter //
create procedure ActivateUser(
	IN email varchar(300),
    IN activationKey varchar(10)
)
begin
	declare _Active bool;
    declare _UsrId int;
    
    if not exists (
		select *
		from Users_Active 
		join Users on Usr_UsrId = UAc_UsrId
			where UAc_ActivationKey = activationKey and
				 Usr_Email = email
	) then
		signal sqlstate '45000'
			set message_text = 'Wrong activation key!';
	end if;
    
	select UAc_Active, Usr_UsrId into _Active, _UsrId
	from Users_Active 
	join Users on Usr_UsrId = UAc_UsrId
		where UAc_ActivationKey = activationKey and
			 Usr_Email = email;
	
    if _Active = true then
		signal sqlstate '45000'
			set message_text = 'User already activated!';
	end if;
            
	update Users_Active
	set UAc_Active = 1 
		where UAc_UsrId = _UsrId;

end 

delimiter //
create procedure GetUserInfo(
	IN email varchar(300)
)
begin
	select * 
    from Users 
    join Users_Active on UAc_UsrId = Usr_UsrId
    where Usr_Email =  email 
    limit 1;
end

DELIMITER //
create procedure RegisterUser(
IN email varchar(300),
IN _hash varchar(9999),
IN _name varchar(100),
IN surname varchar(100),
IN activation_key varchar(10)
)
begin
insert into Users(Usr_Email, Usr_Hash, Usr_Name, Usr_Surname) 
	values (email, _hash, _name, surname);
    
insert into Users_Active(UAc_UsrId, UAc_Active, UAc_ActivationKey)
	select Usr_UsrId, 0, activation_key from Users where Usr_Email = email;

end

DELIMITER //
create procedure CreateGroup(
	IN groupName varchar(100),
    IN email varchar(300)
)
begin
	declare Grp_GrpId int;
    
    if exists(
		select * from `Groups` where Grp_Name = groupName
	) then
		signal sqlstate '45000'
			set message_text = 'There is already an existing group with that name!';
	end if;
    
    insert into `Groups` (Grp_Name) values (groupName);
    select last_insert_id() into Grp_GrpId;
    
    insert into Groups_Members(GMe_GrpId, GMe_UsrId, GMe_Role)
		select Grp_GrpId, Usr_UsrId, 'Owner' from Users where Usr_Email = email;
end

delimiter //
drop procedure RenewJoiningKey;
create procedure RenewJoiningKey(
	IN email varchar(300),
    IN groupName varchar(100),
    IN joiningKey varchar(20)
)
begin
	if not exists (
		select * from Groups_Members
			join `Groups` on Grp_GrpId = GMe_GrpId
            join Users on Usr_UsrId = GMe_UsrId
		where Grp_Name = groupName and
			Usr_Email = email and
			Grp_Name = groupName  and
            GMe_Role = 'Owner'
    ) then
		signal sqlstate '45000'
			set message_text = 'You do not have access to this!';
	end if;
    
    if exists (
		select * from Groups_Join
        where GJo_JoiningKey = joiningKey
			and GJo_ExpirationDate >= current_timestamp()
    ) then
		signal sqlstate '45000'
			set message_text = 'Joining key like that already exists';
	end if;
    
    insert into Groups_Join(GJo_GrpId, GJo_JoiningKey, GJo_ExpirationDate)
		select Grp_GrpId, joiningKey, current_timestamp() + interval 1 day from `Groups` where Grp_Name = groupName;
end


delimiter //
drop procedure JoinGroup;
create procedure JoinGroup(
	IN email varchar(300),
    IN joiningKey varchar(20)
)
begin
	declare GrpId int;

	if not exists (
		select * from Groups_Join
        where GJo_JoiningKey = joiningKey
			and GJo_ExpirationDate >= current_timestamp()
    ) then
		signal sqlstate '45000'
			set message_text = 'Wrong joining key!';
	end if;
    
    select GJo_GrpId into GrpId from Groups_Join
		where GJo_JoiningKey = joiningKey;
        
	if exists (
		select * from Groups_Members
			join Users on Usr_UsrId = GMe_UsrId
        where GMe_GrpId = GrpId and
			Usr_Email = email
    ) then
		signal sqlstate '45000'
			set message_text = 'You are already part of this group!';
	end if;

	insert into Groups_Members(GMe_GrpId, GMe_UsrId, GMe_Role)
		select GrpId, Usr_UsrId, 'User' from Users where Usr_Email = email;

	select Grp_Name from `Groups` where Grp_GrpId = GrpId;
end

delimiter //
drop procedure GetMyGroups;
create procedure GetMyGroups(
	IN email varchar(300)
)
begin
	select Grp_Name, GMe_Role from Groups_Members
		join `Groups` on Grp_GrpId = GMe_GrpId
        join Users on Usr_UsrId = GMe_UsrId
	where Usr_Email = email;

end

delimiter ;;
drop procedure CreateChannel;
create procedure CreateChannel(
	IN groupName varchar(100),
    IN ConId int,
    IN channelName varchar(300)
)
begin
    if exists(
		select * from Channels where Cha_Name = channelName
	) then
		signal sqlstate '45000'
			set message_text = 'There is already an existing channel with that name!';
	end if;
    
    insert into Channels (Cha_GrpId, Cha_ConId, Cha_Name) 
		select Grp_GrpId, ConId, channelName from `Groups` where Grp_Name = groupName;
end

delimiter ;;
drop procedure GetChannelName;
create procedure GetChannelName(
	IN email varchar(300),
	IN groupName varchar(100),
    IN ConId int
)
begin
	if not exists (
		select * from Groups_Members
			join `Groups` on Grp_GrpId = GMe_GrpId
            join Users on Usr_UsrId = GMe_UsrId
		where Grp_Name = groupName and
			Usr_Email = email and
			Grp_Name = groupName
    ) then
		signal sqlstate '45000'
			set message_text = 'You do not have access to this!';
	end if;
    
   select Cha_Name from Channels
	join `Groups` on Grp_GrpId = Cha_GrpId
    where Grp_Name = groupName 
		and Cha_ConId = ConId;
end
    

delimiter //
drop procedure GetMyConferences;
create procedure GetMyConferences(
	IN email varchar(300)
)
begin
	select Con_ConId, Grp_Name, Con_Topic, Con_StartDate, Con_Description from Groups_Members
		join `Groups` on Grp_GrpId = GMe_GrpId
        join Users on Usr_UsrId = GMe_UsrId
        join Conferences on Con_GrpId = Grp_GrpId
	where Usr_Email = email and
		Con_StartDate > DATE_SUB(NOW(), INTERVAL 12 HOUR)
	order by Con_StartDate;
end