DELIMITER $$
USE `application-authorizations`$$
CREATE PROCEDURE `InsertOperation`(
	IN _ApplicationID BIGINT,
	IN _Name VARBINARY(50),
	IN _Enabled BIT
)
BEGIN
	INSERT INTO `application-authorizations`.`operations`
	(
		`ApplicationID`,
		`Name`,
		`Enabled`,
		`Created`,
		`Updated`
	)
	VALUES
	(
		_ApplicationID,
		_Name,
		_Enabled,
		UTC_Timestamp(),
		UTC_Timestamp()
	);
	
	SELECT LAST_INSERT_ID() as `ID`;
END$$

DELIMITER ;