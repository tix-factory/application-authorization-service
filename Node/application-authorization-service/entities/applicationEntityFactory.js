const MaxApplicationNameLength = 128;
const ValidationRegex = /^\s+$/;

const isApplicationNameValid = (applicationName) => {
	if (typeof(applicationName) !== "string") {
		return false;
	}

	return applicationName && !ValidationRegex.test(applicationName) && applicationName.length <= MaxApplicationNameLength;
};

export default class {
	constructor(applicationsCollection) {
		this.applicationsCollection = applicationsCollection;
	}

	async setup() {
		await this.applicationsCollection.createIndex({
			"name": 1
		}, {
			unique: true
		});
	}

	async getOrCreateApplicationByName(applicationName) {
		if (!isApplicationNameValid(applicationName)) {
			return Promise.reject("InvalidApplicationName");
		}

		let application = await this.getApplicationByName(applicationName);
		if (application) {
			return Promise.resolve(application);
		}

		const applicationId = await this.applicationsCollection.insert({
			name: applicationName
		});

		return this.applicationsCollection.findOne({
			id: applicationId
		});
	}

	getApplicationById(applicationId) {
		if (typeof(applicationId) !== "number" || isNaN(applicationId) || applicationId <= 0) {
			return Promise.resolve(null);
		}

		return this.applicationsCollection.findOne({
			id: applicationId
		});
	}

	getApplicationByName(applicationName) {
		if (!isApplicationNameValid(applicationName)) {
			return Promise.resolve(null);
		}

		return this.applicationsCollection.findOne({
			name: applicationName
		});
	}
};
