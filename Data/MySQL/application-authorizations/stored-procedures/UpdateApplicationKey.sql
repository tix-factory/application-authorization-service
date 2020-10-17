DELIMITER $$
USE `application-authorizations`$$
CREATE PROCEDURE `UpdateApplicationKey`(
	IN _ApplicationID BIGINT,
	IN _KeyHash VARBINARY(64),
	IN _Name VARBINARY(50),
	IN _Enabled BIT,
	IN _ID BIGINT
)
BEGIN
	UPDATE `application-authorizations`.`application-keys`
	SET
		`ApplicationID` = _ApplicationID,
		`Name` = _Name,
		`KeyHash` = _KeyHash,
		`Enabled` = _Enabled,
		`Updated` = UTC_Timestamp()
	WHERE (`ID` = _ID)
	LIMIT 1;
END$$

DELIMITER ;