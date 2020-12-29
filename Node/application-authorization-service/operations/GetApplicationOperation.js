export default class {
	constructor(applicationEntityFactory, operationEntityFactory) {
		this.applicationEntityFactory = applicationEntityFactory;
		this.operationEntityFactory = operationEntityFactory;
	}

    get name() {
        return "GetApplication";
    }
	
	get requestParameters() {
		return ["data"];
	}
 
    async execute(requestBody) {
		const application = await this.applicationEntityFactory.getOrCreateApplicationByName(requestBody.data);
		if (!application) {
			return Promise.resolve(null);
		}

		const operations = await this.operationEntityFactory.getOperationsByApplicationName(application.name);
		return Promise.resolve({
			name: application.name,
			operations: operations.map(o => {
				return {
					name: o.name,
					enabled: o.enabled,
					created: o.created,
					updated: o.updated
				}
			}),
			created: application.created,
			updated: application.updated
		});
    }
};