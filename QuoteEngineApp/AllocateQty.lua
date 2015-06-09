var qty = VARGS[1];
var allocateFrom = KEYS[2];
var quoteId = KEYS[3];
var price = KEYS[4];

if redis.hget(allocateFrom, "Allocated") >= qty then
    redis.call("hincrby", allocateFrom, "Allocated", qty*-1)
    redis.call("hmset", quoteId, "Qty", qty, "Price", price, "ExpTime", redis.call("time") )
    return qty
else
    return 0
end
