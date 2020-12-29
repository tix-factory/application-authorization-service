import { dirname } from "path";
import { fileURLToPath } from 'url';
import { HttpServer } from "@tix-factory/http-service";
import { MongoConnection } from "@tix-factory/mongodb";
import { v4 as GenerateGuid } from "uuid";

import ApplicationEntityFactory from "./entities/applicationEntityFactory.js";
import ApplicationKeyEntityFactory from "./entities/applicationKeyEntityFactory.js";
import OperationEntityFactory from "./entities/operationEntityFactory.js";
import ApplicationOperationAuthorizationEntityFactory from "./entities/applicationOperationAuthorizationEntityFactory.js";

import AuthorizationHandler from "./implementation/authorizationHandler.js";
import KeyHasher from "./implementation/keyHasher.js";

import GetApplicationOperation from "./operations/GetApplicationOperation.js";
import GetAuthorizedOperationsOperation from "./operations/GetAuthorizedOperationsOperation.js";
import RegisterApplicationOperation from "./operations/RegisterApplicationOperation.js";
import RegisterOperationOperation from "./operations/RegisterOperationOperation.js";
import WhoAmIOperation from "./operations/WhoAmIOperation.js";

const setupKeyName = "Application Setup";
const workingDirectory = dirname(fileURLToPath(import.meta.url));

const service = new HttpServer({
    name: "TixFactory.ApplicationAuthorization.Service",
    logName: "TFAAS2.TixFactory.ApplicationAuthorization.Service"
});

const getOrCreateSetupKey = async (application, applicationKeyEntityFactory) => {
	let setupKey = await applicationKeyEntityFactory.getApplicationKeyByApplicationNameAndKeyName(application.name, setupKeyName);
	if (setupKey) {
		return Promise.resolve(setupKey);
	}

	const setupKeyGuid = GenerateGuid();
	setupKey = await applicationKeyEntityFactory.createApplicationKey(application.name, setupKeyName, setupKeyGuid);

	console.log(`Setup ApiKey (${setupKeyName}): ${setupKeyGuid}`);

	return Promise.resolve(setupKey);
};

const registerApplication = async (applicationEntityFactory, operationEntityFactory, applicationOperationAuthorizationEntityFactory, operationRegistry) => {
	const application = await applicationEntityFactory.getOrCreateApplicationByName(service.options.name);

	await Promise.all(operationRegistry.operations.map(operation => {
		if (operation.allowAnonymous) {
			return Promise.resolve();
		}
	
		return new Promise(async (resolve, reject) => {
			try {
				await operationEntityFactory.getOrCreateOperation(application.name, operation.name);

				// Allow TixFactory.ApplicationAuthorization.Service to access its own operations
				// This allows the logged setup key to be used with this service.
				await applicationOperationAuthorizationEntityFactory.createAuthorization(application.name, application.name, operation.name);

				resolve();
			} catch (e) {
				reject(e);
			}
		});
	}));

	return Promise.resolve(application);
};

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
			
			const authorizationHandler = service.authorizationHandler = new AuthorizationHandler(service.logger, service.options.name, applicationEntityFactory, applicationKeyEntityFactory, applicationOperationAuthorizationEntityFactory);
			
			service.operationRegistry.registerOperation(new GetApplicationOperation(applicationEntityFactory, operationEntityFactory));
			service.operationRegistry.registerOperation(new GetAuthorizedOperationsOperation(service.logger, authorizationHandler, applicationEntityFactory, applicationKeyEntityFactory));
			service.operationRegistry.registerOperation(new RegisterApplicationOperation(applicationEntityFactory));
			service.operationRegistry.registerOperation(new RegisterOperationOperation(operationEntityFactory));
			service.operationRegistry.registerOperation(new WhoAmIOperation(service.logger, applicationEntityFactory, applicationKeyEntityFactory));

			const runningApplication = await registerApplication(applicationEntityFactory, operationEntityFactory, applicationOperationAuthorizationEntityFactory, service.operationRegistry);
			await getOrCreateSetupKey(runningApplication, applicationKeyEntityFactory);

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
