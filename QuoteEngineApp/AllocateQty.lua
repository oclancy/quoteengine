var qty = KEYS[1];
var allocateFrom = KEYS[2];
var quoteRequestId = KEYS[3];
var quoteId = KEYS[3];

if( redis.hget(allocateFrom, "ALLOCATED") >= qty )
	redis.hset