export default class {
	constructor(applicationEntityFactory) {
		this.applicationEntityFactory = applicationEntityFactory;
	}

    get name() {
        return "RegisterApplication";
    }
	
	get requestParameters() {
		return ["name"];
	}
 
    async execute(requestBody) {
		await this.applicationEntityFactory.getOrCreateApplicationByName(requestBody.name);
		return Promise.resolve();
    }
};