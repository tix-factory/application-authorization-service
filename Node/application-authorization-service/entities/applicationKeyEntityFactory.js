const MaxApplicationKeyNameLength = 128;

const isApplicationKeyNameValid = (applicationKeyName) => {
	if (typeof(applicationKeyName) !== "string") {
		return false;
	}

	return applicationName && applicationName.length <= MaxApplicationKeyNameLength;
};

export default class {
	constructor(applicationEntityFactory, applicationKeysCollection, keyHasher) {
		this.applicationEntityFactory = applicationEntityFactory;
		this.applicationKeysCollection = applicationKeysCollection;
		this.keyHasher = keyHasher;
	}

	async setup() {
		await this.applicationKeysCollection.createIndex({
			"applicationId": 1,
			"name": 1
		}, {
			unique: true
		});

		await this.applicationKeysCollection.createIndex({
			"keyHash": 1
		}, {
			unique: true,

			// This index is case-sensitive.
			collation: undefined
		});
	}

	async createApplicationKey(applicationName, name, guid) {
		if (!isApplicationKeyNameValid(name)) {
			return Promise.reject("InvalidKeyName");
		}

		if (!this.keyHasher.validate(guid)) {
			return Promise.reject("InvalidKey");
		}

		const keyHash = await this.keyHasher.hash(guid);

		const application = await this.applicationEntityFactory.getApplicationByName(applicationName);
		if (!application) {
			return Promise.reject("InvalidApplicationName");
		}

		const applicationKeyId = await this.applicationKeysCollection.insert({
			applicationId: application.id,
			name: name,
			keyHash: keyHash,
			enabled: true
		});

		return this.applicationKeysCollection.findOne({
			id: applicationKeyId
		});
	}

	async getApplicationKeyByApplicationNameAndKeyName(applicationName, applicationKeyName) {
		if (!isApplicationKeyNameValid(applicationKeyName)) {
			return Promise.resolve(null);
		}

		const application = await this.applicationEntityFactory.getApplicationByName(applicationName);
		if (!application) {
			return Promise.resolve(null);
		}

		return this.applicationKeysCollection.findOne({
			applicationId: application.id,
			name: applicationKeyName
		});
	}

	async getApplicationKeyByGuid(guid) {
		if (!this.keyHasher.validate(guid)) {
			return Promise.resolve(null);
		}

		const keyHash = await this.keyHasher.hash(guid);
		return this.applicationKeysCollection.findOne({
			keyHash: keyHash
		}, {
			// This lookup is case-sensitive.
			collation: undefined
		});
	}
};
