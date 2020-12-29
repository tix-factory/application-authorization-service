export default class {
	constructor(applicationOperationAuthorizationsCollection, applicationEntityFactory, operationEntityFactory) {
		this.applicationOperationAuthorizationsCollection = applicationOperationAuthorizationsCollection;
		this.applicationEntityFactory = applicationEntityFactory;
		this.operationEntityFactory = operationEntityFactory;
	}

	async setup() {
		await this.applicationOperationAuthorizationsCollection.createIndex({
			"applicationId": 1,
			"operationId": 1
		}, {
			unique: true
		});
	}

	async createAuthorization(applicationName, targetApplicationName, targetOperationName) {
		const application = await this.applicationEntityFactory.getApplicationByName(applicationName);
		if (!application) {
			return Promise.reject("InvalidApplicationName");
		}

		const operation = await this.operationEntityFactory.getOperationByApplicationNameAndOperationName(targetApplicationName, targetOperationName);
		if (!operation) {
			return Promise.reject("InvalidOperationName");
		}

		const authorization = await this.applicationOperationAuthorizationsCollection.findOne({
			applicationId: application.id,
			operationId: operation.id
		});

		if (authorization) {
			return Promise.resolve(false);
		}

		await this.applicationOperationAuthorizationsCollection.insert({
			applicationId: application.id,
			operationId: operation.id
		});

		return Promise.resolve(true);
	}

	async deleteAuthorization(applicationName, targetApplicationName, targetOperationName) {
		const application = await this.applicationEntityFactory.getApplicationByName(applicationName);
		if (!application) {
			return Promise.reject("InvalidApplicationName");
		}

		const operation = await this.operationEntityFactory.getOperationByApplicationNameAndOperationName(targetApplicationName, targetOperationName);
		if (!operation) {
			return Promise.reject("InvalidOperationName");
		}

		return this.applicationOperationAuthorizationsCollection.deleteOne({
			applicationId: application.id,
			operationId: operation.id
		});
	}

	async getAuthorizedOperationsByApplication(application) {
		const authorizationEntities = await this.applicationOperationAuthorizationsCollection.find({
			applicationId: application.id
		});

		const operations = [];
		await Promise.all(authorizationEntities.map(e => {
			return this.operationEntityFactory.getOperationById(e.operationId).then(operation => {
				if (operation) {
					operations.push(operation);
				}
			});
		}));

		return Promise.resolve(operation);
	}
};
