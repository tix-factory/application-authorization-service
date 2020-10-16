DELIMITER $$
USE `application-authorizations`$$
CREATE PROCEDURE `InsertService`(
	IN _Name VARBINARY(50)
)
BEGIN
	INSERT INTO `application-authorizations`.`services`
	(
		`Name`,
		`Created`,
		`Updated`
	)
	VALUES
	(
		_Name,
		UTC_Timestamp(),
		UTC_Timestamp()
	);
	
	SELECT LAST_INSERT_ID() as `ID`;
END$$

DELIMITER ;