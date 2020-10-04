DELIMITER $$
USE `application-authorizations`$$
CREATE PROCEDURE `DeleteService`(
	IN _ID BIGINT
)
BEGIN
	DELETE
		FROM `application-authorizations`.`services`
		WHERE (`ID` = _ID)
		LIMIT 1;
END$$

DELIMITER ;