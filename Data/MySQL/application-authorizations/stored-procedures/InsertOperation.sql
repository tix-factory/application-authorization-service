DELIMITER $$
USE `application-authorizations`$$
CREATE PROCEDURE `InsertOperation`(
	IN _ServiceID BIGINT,
	IN _Name VARBINARY(50),
	IN _Enabled BIT
)
BEGIN
	INSERT INTO `application-authorizations`.`operations`
	(
		`ServiceID`,
		`Name`,
		`Enabled`,
		`Created`,
		`Updated`
	)
	VALUES
	(
		_ServiceID,
		_Name,
		_Enabled,
		UTC_Timestamp(),
		UTC_Timestamp()
	);
	
	SELECT LAST_INSERT_ID() as `ID`;
END$$

DELIMITER ;