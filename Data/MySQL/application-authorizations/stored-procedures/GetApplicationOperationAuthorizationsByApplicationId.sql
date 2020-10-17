DELIMITER $$
USE `application-authorizations`$$
CREATE PROCEDURE `GetApplicationOperationAuthorizationsByApplicationId`(
	IN _ApplicationID BIGINT,
	IN _Count INT
)
BEGIN
	SELECT *
		FROM `application-authorizations`.`application-operation-authorizations`
		WHERE `ApplicationID` = _ApplicationID
		LIMIT _Count;
END$$

DELIMITER ;