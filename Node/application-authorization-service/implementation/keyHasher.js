import crypto from "crypto";

const CacheExpiry = 10 * 60 * 1000;
const GuidRegex = /^[0-9a-f]{8}-[0-9a-f]{4}-[0-5][0-9a-f]{3}-[089ab][0-9a-f]{3}-[0-9a-f]{12}$/i;

const guidToBytes = guid => {
	// https://gist.github.com/daboxu/4f1dd0a254326ac2361f8e78f89e97ae
	const bytes = [];
    guid.split('-').map((number, index) => {
        const bytesInChar = index < 3 ? number.match(/.{1,2}/g).reverse() :  number.match(/.{1,2}/g);
        bytesInChar.map((byte) => { bytes.push(parseInt(byte, 16));})
	});

    return bytes;
};

export default class {
	constructor() {
		this.hashCache = {};
	}

	validate(guid) {
		return guid && GuidRegex.test(guid);
	}

	async hash(guid) {
		if (!this.validate(guid)) {
			return Promise.reject(new Error(`guid input is not guid format`));
		}

		const cachedHash = this.hashCache[guid];
		if (cachedHash) {
			return Promise.resolve(cachedHash);
		}

		// https://stackoverflow.com/a/48161723/1663648
		// https://stackoverflow.com/a/27970509/1663648
		const guidBytes = guidToBytes(guid);
		const guidBuffer = Buffer.from(guidBytes);
		const hash = crypto.createHash("sha256");
		hash.update(guidBuffer);

		const sha256Hash = hash.digest("hex").toUpperCase();
		this.hashCache[guid] = sha256Hash;

		setTimeout(() => {
			delete this.hashCache[guid];
		}, CacheExpiry);

		return Promise.resolve(sha256Hash);
	}
};
