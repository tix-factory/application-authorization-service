const MaxOperationNameLength = 128;
const ValidationRegex = /^\s+$/;

const isOperationNameValid = (operationName) => {
	if (typeof(operationName) !== "string") {
		return false;
	}

	return operationName && !ValidationRegex.test(operationName) && operationName.length <= MaxOperationNameLength;
};

export default class {
	constructor(operationsCollection, applicationEntityFactory) {
		this.operationsCollection = operationsCollection;
		this.applicationEntityFactory = applicationEntityFactory;
	}

	async setup() {
		await this.operationsCollection.createIndex({
			"applicationId": 1,
			"name": 1
		}, {
			unique: true
		});
	}

	async getOrCreateOperation(applicationName, operationName) {
		if (!isOperationNameValid(applicationName)) {
			return Promise.reject("InvalidOperationName");
		}

		const application = await this.applicationEntityFactory.getApplicationByName(applicationName);
		if (!application) {
			return Promise.reject("InvalidApplicationName");
		}

		const operation = await this.operationsCollection.findOne({
			applicationId: application.id,
			name: operationName
		});
		
		if (operation) {
			return Promise.resolve(operation);
		}

		const operationId = await this.operationsCollection.insert({
			applicationId: application.id,
			name: operationName
		});

		return this.operationsCollection.findOne({
			id: operationId
		});
	}

	async getOperationsByApplicationName(applicationName) {
		const application = await this.applicationEntityFactory.getApplicationByName(applicationName);
		if (!application) {
			return Promise.reject("InvalidApplicationName");
		}

		return this.operationsCollection.find({
			applicationId: application.id
		});
	}

	async getOperationByApplicationNameAndOperationName(applicationName, operationName) {
		if (!isOperationNameValid(applicationName)) {
			return Promise.resolve(null);
		}

		const application = await this.applicationEntityFactory.getApplicationByName(applicationName);
		if (!application) {
			return Promise.resolve(null);
		}

		return this.operationsCollection.findOne({
			applicationId: application.id,
			name: operationName
		});
	}

	getOperationById(operationId) {
		if (typeof(operationId) !== "number" || isNaN(operationId) || operationId <= 0) {
			return Promise.resolve(null);
		}

		return this.operationsCollection.findOne({
			id: operationId
		});
	}
};
