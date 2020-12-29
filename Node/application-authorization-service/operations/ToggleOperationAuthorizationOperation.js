export default class {
	constructor(applicationOperationAuthorizationEntityFactory) {
		this.applicationOperationAuthorizationEntityFactory = applicationOperationAuthorizationEntityFactory;
	}

    get name() {
        return "ToggleOperationAuthorization";
    }
	
	get requestParameters() {
		return ["applicationName", "targetApplicationName", "targetOperationName", "authorized"];
	}
 
    async execute(requestBody) {
		if (requestBody.authorized) {
			await this.applicationOperationAuthorizationEntityFactory.createAuthorization(requestBody.applicationName, requestBody.targetApplicationName, requestBody.targetOperationName);
		} else {
			await this.applicationOperationAuthorizationEntityFactory.deleteAuthorization(requestBody.applicationName, requestBody.targetApplicationName, requestBody.targetOperationName);
		}

		return Promise.resolve();
    }
};