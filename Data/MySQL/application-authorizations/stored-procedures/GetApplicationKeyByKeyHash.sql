DELIMITER $$
USE `application-authorizations`$$
CREATE PROCEDURE `GetApplicationKeyByKeyHash`(
	IN _KeyHash VARBINARY(64)
)
BEGIN
	SELECT *
		FROM `application-authorizations`.`application-keys`
		WHERE `KeyHash` = _KeyHash
		LIMIT 1;
END$$

DELIMITER ;