DELIMITER $$
USE `application-authorizations`$$
CREATE PROCEDURE `InsertApplicationKey`(
	IN _ApplicationID BIGINT,
	IN _Name VARBINARY(50),
	IN _KeyHash VARBINARY(64),
	IN _Enabled BIT
)
BEGIN
	INSERT INTO `application-authorizations`.`application-keys`
	(
		`ApplicationID`,
		`Name`,
		`KeyHash`,
		`Enabled`,
		`Created`,
		`Updated`
	)
	VALUES
	(
		_ApplicationID,
		_Name,
		_KeyHash,
		_Enabled,
		UTC_Timestamp(),
		UTC_Timestamp()
	);
	
	SELECT LAST_INSERT_ID() as `ID`;
END$$

DELIMITER ;