import { dirname } from "path";
import { fileURLToPath } from 'url';
import { HttpServer } from "@tix-factory/http-service";
import { MongoConnection } from "@tix-factory/mongodb";

import ApplicationEntityFactory from "./entities/applicationEntityFactory.js";
import ApplicationKeyEntityFactory from "./entities/applicationKeyEntityFactory.js";
import OperationEntityFactory from "./entities/operationEntityFactory.js";
import ApplicationOperationAuthorizationEntityFactory from "./entities/applicationOperationAuthorizationEntityFactory.js";

import AuthorizationHandler from "./implementation/authorizationHandler.js";
import KeyHasher from "./implementation/keyHasher.js";

/*
import DeleteApplicationSettingOperation from "./operations/DeleteApplicationSettingOperation.js";
import GetApplicationSettingsOperation from "./operations/GetApplicationSettingsOperation.js";
import SetApplicationSettingOperation from "./operations/SetApplicationSettingOperation.js";
import SetApplicationSettingValueOperation from "./operations/SetApplicationSettingValueOperation.js";
*/

const workingDirectory = dirname(fileURLToPath(import.meta.url));

const service = new HttpServer({
    name: "TixFactory.ApplicationAuthorization.Service",
    logName: "TFAAS2.TixFactory.ApplicationAuthorization.Service"
});

const init = () => {
	console.log(`Starting ${service.options.name}...\n\tWorking directory: ${workingDirectory}\n\tNODE_ENV: ${process.env.NODE_ENV}\n\tPort: ${service.options.port}`);

	return new Promise(async (resolve, reject) => {
		try {
			const mongoConnection = new MongoConnection(process.env.MongoConnectionString);
			const applicationsCollection = await mongoConnection.getCollection("application-authorization-service", "applications");
			const applicationKeysCollection = await mongoConnection.getCollection("application-authorization-service", "application-keys");
			const operationsCollection = await mongoConnection.getCollection("application-authorization-service", "operations");
			const applicationOperationAuthorizationsCollection = await mongoConnection.getCollection("application-authorization-service", "application-operation-authorizations", {
				// collation doesn't make sense here, there's no strings
				collation: undefined
			});

			const keyHasher = new KeyHasher();
			service.authorizationHandler = new AuthorizationHandler(service.logger);

			const applicationEntityFactory = new ApplicationEntityFactory(applicationsCollection);
			const applicationKeyEntityFactory = new ApplicationKeyEntityFactory(applicationEntityFactory, applicationKeysCollection, keyHasher);
			const operationEntityFactory = new OperationEntityFactory(operationsCollection, applicationEntityFactory);
			const applicationOperationAuthorizationEntityFactory = new ApplicationOperationAuthorizationEntityFactory(applicationOperationAuthorizationsCollection, applicationEntityFactory, operationEntityFactory);

			await Promise.all([
				applicationEntityFactory.setup(),
				applicationKeyEntityFactory.setup(),
				operationEntityFactory.setup(),
				applicationOperationAuthorizationEntityFactory.setup()
			]);
			
			/*
			service.operationRegistry.registerOperation(new DeleteApplicationSettingOperation(settingEntityFactory));
			service.operationRegistry.registerOperation(new GetApplicationSettingsOperation(settingEntityFactory, applicationNameProvider));
			service.operationRegistry.registerOperation(new SetApplicationSettingOperation(settingEntityFactory));
			service.operationRegistry.registerOperation(new SetApplicationSettingValueOperation(settingEntityFactory, applicationNameProvider));
			*/

			resolve();
		} catch (e) {
			reject(e);
		}
	});
};

init().then(() => {
	service.listen();
}).catch(err => {
	service.logger.error(err);
	console.error(err);
	process.exit(1);
});
