DELIMITER $$
USE `application-authorizations`$$
CREATE PROCEDURE `GetOperationsByApplicationId`(
	IN _ApplicationID BIGINT,
	IN _Count INTEGER
)
BEGIN
	SELECT *
		FROM `application-authorizations`.`operations`
		WHERE `ApplicationID` = _ApplicationID
		LIMIT _Count;
END$$

DELIMITER ;