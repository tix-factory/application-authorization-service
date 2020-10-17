DELIMITER $$
USE `application-authorizations`$$
CREATE PROCEDURE `GetApplications`(
	IN _Count INTEGER
)
BEGIN
	SELECT *
		FROM `application-authorizations`.`applications`
		LIMIT _Count;
END$$

DELIMITER ;