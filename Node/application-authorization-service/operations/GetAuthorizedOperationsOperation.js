export default class {
	constructor(logger, authorizationHandler, applicationEntityFactory, applicationKeyEntityFactory) {
		this.logger = logger;
		this.authorizationHandler = authorizationHandler;
		this.applicationEntityFactory = applicationEntityFactory;
		this.applicationKeyEntityFactory = applicationKeyEntityFactory;
	}

	get allowAnonymous() {
		return true;
	}

    get name() {
        return "GetAuthorizedOperations";
    }
	
	get requestParameters() {
		return ["apiKey"];
	}
 
    async execute(requestBody, request) {
		const targetApplicationKey = await this.applicationKeyEntityFactory.getApplicationKeyByGuid(request.apiKey);
		if (!targetApplicationKey) {
			return Promise.resolve([]);
		}

		const targetApplication = await this.applicationEntityFactory.getApplicationById(targetApplicationKey.applicationId);
		if (!targetApplication) {
			this.logger.warn("Valid ApiKey does not map to valid application.",
				"\n\tKey:", targetApplicationKey.name, `(${targetApplicationKey.id})`);

			return Promise.resolve([]);
		}

		const authorizedOperationNames = await this.authorizationHandler.loadAndCacheAuthorizedOperations(targetApplication.name, requestBody.apiKey);
		return Promise.resolve(authorizedOperationNames);
    }
};