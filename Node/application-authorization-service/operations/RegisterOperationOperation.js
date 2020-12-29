export default class {
	constructor(operationEntityFactory) {
		this.operationEntityFactory = operationEntityFactory;
	}

    get name() {
        return "RegisterOperation";
    }
	
	get requestParameters() {
		return ["applicationName", "operationName"];
	}
 
    async execute(requestBody) {
		await this.operationEntityFactory.getOrCreateOperation(requestBody.applicationName, requestBody.operationName);
		return Promise.resolve();
    }
};