import { v4 as GenerateGuid } from "uuid";

export default class {
	constructor(applicationKeyEntityFactory) {
		this.applicationKeyEntityFactory = applicationKeyEntityFactory;
	}

    get name() {
        return "CreateApplicationKey";
    }
	
	get requestParameters() {
		return ["applicationName", "keyName"];
	}
 
    async execute(requestBody) {
		const guid = GenerateGuid();
		await this.applicationKeyEntityFactory.createApplicationKey(requestBody.applicationName, requestBody.keyName, guid);

		return Promise.resolve(guid);
    }
};