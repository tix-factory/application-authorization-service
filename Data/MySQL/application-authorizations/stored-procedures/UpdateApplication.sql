DELIMITER $$
USE `application-authorizations`$$
CREATE PROCEDURE `UpdateApplication`(
	IN _Name VARBINARY(50),
	IN _ID BIGINT
)
BEGIN
	UPDATE `application-authorizations`.`applications`
	SET
		`Name` = _Name,
		`Updated` = UTC_Timestamp()
	WHERE (`ID` = _ID)
	LIMIT 1;
END$$

DELIMITER ;