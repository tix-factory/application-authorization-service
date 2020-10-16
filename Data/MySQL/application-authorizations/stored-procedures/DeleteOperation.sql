DELIMITER $$
USE `application-authorizations`$$
CREATE PROCEDURE `DeleteOperation`(
	IN _ID BIGINT
)
BEGIN
	DELETE
		FROM `application-authorizations`.`operations`
		WHERE (`ID` = _ID)
		LIMIT 1;
END$$

DELIMITER ;