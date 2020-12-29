import { validate as isGuid } from "uuid";

const CacheExpiryInMilliseconds = 60 * 1000;

export default class {
	constructor(logger, serviceName, applicationEntityFactory, applicationKeyEntityFactory, applicationOperationAuthorizationEntityFactory) {
		this.logger = logger;
		this.serviceName = serviceName;
		this.applicationEntityFactory = applicationEntityFactory;
		this.applicationKeyEntityFactory = applicationKeyEntityFactory;
		this.applicationOperationAuthorizationEntityFactory = applicationOperationAuthorizationEntityFactory;
		this.cache = {};
	}

	isAuthorized(apiKey, operation) {
		if (operation.allowAnonymous) {
			return Promise.resolve(true);
		}

		return new Promise(async (resolve, reject) => {
			try {
				const authorizedOperations = await this.getAuthorizedOperations(this.serviceName, apiKey);
				resolve(authorizedOperations.map(o => o.toLowerCase()).includes(operation.name.toLowerCase()));
			} catch (e) {
				this.logger.error(`Failed to load application authorizations for ApiKey.\n`, e);
				resolve(false);
			}
		});
	}

	getAuthorizedOperations(targetApplicationName, apiKey) {
		if (!isGuid(apiKey)) {
			return Promise.resolve([]);
		}

		const currentTime = +new Date;
		const cacheKey = `${targetApplicationName}:${apiKey}`;

		let cachedAuthorizations = this.cache[cacheKey];
		if (cachedAuthorizations) {
			cachedAuthorizations.accessExpiry = currentTime + (CacheExpiryInMilliseconds * 2);
			if (cachedAuthorizations.refreshExpiry < currentTime) {
				cachedAuthorizations.refreshExpiry = currentTime + CacheExpiryInMilliseconds;
				setTimeout(() => this.refreshAuthorizedOperations(targetApplicationName, apiKey), 0);
			}

			return Promise.resolve(cachedAuthorizations.operations);
		}

		return this.loadAndCacheAuthorizedOperations(targetApplicationName, apiKey);
	}

	refreshAuthorizedOperations(targetApplicationName, apiKey) {
		this.loadAndCacheAuthorizedOperations(targetApplicationName, apiKey).then(() => {
			// Loading the authorizations also cached them.
		}).catch(err => {
			this.logger.warn(`Failed to refresh application authorizations for ApiKey.\n`, err);
		});
	}

	loadAndCacheAuthorizedOperations(targetApplicationName, apiKey) {
		return new Promise(async (resolve, reject) => {
			try {
				const authorizedOperations = await this.loadAuthorizedOperations(targetApplicationName, apiKey);
				this.cacheAuthorizations(targetApplicationName, apiKey, authorizedOperations);
	
				resolve(authorizedOperations);
			} catch (e) {
				reject(e);
			}
		});
	}

	loadAuthorizedOperations(targetApplicationName, apiKey) {
		return new Promise(async (resolve, reject) => {
			try {
				const applicationKey = await this.applicationKeyEntityFactory.getApplicationKeyByGuid(apiKey);
				if (!applicationKey?.enabled) {
					resolve([]);
					return;
				}
				
				const apiKeyApplication = await this.applicationEntityFactory.getApplicationById(applicationKey.applicationId);
				if (!apiKeyApplication) {
					this.logger.warn("Valid ApiKey does not map to valid application.",
						"\n\tKey:", applicationKey.name, `(${applicationKey.id})`);
				}

				const authorizedOperations = await this.applicationOperationAuthorizationEntityFactory.getAuthorizedOperationsByApplicationName(apiKeyApplication.name);
				if (authorizedOperations.length < 1) {
					resolve([]);
					return;
				}

				const operationNames = [];
				const targetApplication = await this.applicationEntityFactory.getApplicationByName(targetApplicationName);

				authorizedOperations.forEach(operation => {
					if (operation.applicationId === targetApplication.id) {
						operationNames.push(operation.name);
					}
				});

				resolve(operationNames.sort());
			} catch (e) {
				reject(e);
			}
		});
	}

	cacheAuthorizations(targetApplicationName, apiKey, authorizedOperations) {
		let currentTime = +new Date;
		const cacheKey = `${targetApplicationName}:${apiKey}`;

		this.cache[cacheKey] = {
			accessExpiry: currentTime + (CacheExpiryInMilliseconds * 2),
			refreshExpiry: currentTime + CacheExpiryInMilliseconds,
			operations: authorizedOperations
		};

		setTimeout(() => {
			const cachedAuthorizations = this.cache[cacheKey];
			let currentTime = +new Date;

			if (cachedAuthorizations && cachedAuthorizations.accessExpiry < currentTime) {
				delete this.cache[cacheKey];
			}
		}, CacheExpiryInMilliseconds * 3);
	}
}