DELIMITER $$
USE `application-authorizations`$$
CREATE PROCEDURE `GetApplicationKeysByApplicationId`(
	IN _ApplicationID BIGINT,
	IN _Count INT
)
BEGIN
	SELECT *
		FROM `application-authorizations`.`application-keys`
		WHERE `ApplicationID` = _ApplicationID
		LIMIT _Count;
END$$

DELIMITER ;