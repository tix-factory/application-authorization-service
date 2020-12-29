export default class {
	constructor(logger, applicationEntityFactory, applicationKeyEntityFactory) {
		this.logger = logger;
		this.applicationEntityFactory = applicationEntityFactory;
		this.applicationKeyEntityFactory = applicationKeyEntityFactory;
	}

	get allowAnonymous() {
		return true;
	}

    get name() {
        return "WhoAmI";
    }
 
    async execute(requestBody, request) {
		const applicationKey = await this.applicationKeyEntityFactory.getApplicationKeyByGuid(request.apiKey);
		if (!applicationKey) {
			return Promise.resolve(null);
		}

		const application = await this.applicationEntityFactory.getApplicationById(applicationKey.applicationId);
		if (!application) {
			this.logger.warn("Valid ApiKey does not map to valid application.",
				"\n\tKey:", applicationKey.name, `(${applicationKey.id})`);

			return Promise.resolve(null);
		}

		return Promise.resolve({
			applicationName: application.name,
			applicationKeyName: applicationKey.name
		});
    }
};