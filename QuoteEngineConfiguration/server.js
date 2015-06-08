/**
 * Module dependencies.
 */

var express = require('express');
var routes = require('./routes');
var http = require('http');
var path = require('path');
var bodyParser = require('body-parser');
var Redis = require('ioredis');

var redis = new Redis();
var app = express();

// all environments
app.set('port', process.env.PORT || 3000);
app.set('views', path.join(__dirname, 'views'));
app.set('view engine', 'jade');
//app.use(express.favicon());
//app.use(logger('dev'));
app.use(bodyParser.json());
app.use(bodyParser.urlencoded());
//app.use(express.methodOverride());
//app.use(app.router);
//app.use(require('stylus').middleware(path.join(__dirname, 'public')));
app.use(express.static(path.join(__dirname, 'public')));

// development only
if ('development' == app.get('env')) {
  //app.use(express.errorHandler());
}

app.route('/')
   .get(function(req, res) {
        routes.index(req,res);
    });

app.route('/data/:marketMaker')
   .get(function(req, res) {

       var retval ={
           symbols:[],
           priorities:[],
           clients:[]
           };

       var marketMaker = req.params.marketMaker;

       redis.keys( marketMaker + ':InstrumentConfig:*' )
            .then(function(data){
                data.forEach(function(element, index, array){
                    redis.hgetall(element)
                         .then(function(instrConfig){
                             retval.symbols.push(instrConfig);
                             });
                    });
            });

        res.json(retval);
    });

app.route('/:marketMaker')
   .post(function(req, res) {

       var marketMaker = req.params.marketMaker;

       req.body
             .symbols
             .forEach(function(element, index, array){
                   redis.sadd( 'InstrumentByMarketMaker:' + element.isin + ':' + element.symbol, marketMaker);
                   redis.hmset(marketMaker + ':InstrumentConfig:'+ element.isin + ':' + element.symbol, element);       
            });

        req.body
             .priorities
             .forEach(function(element, index, array){
                   redis.hmset(marketMaker + ':Priorities:' + element.priority, element);       
           });

        req.body
             .clients
             .forEach(function(element, index, array){
                   redis.sadd('MarketMakersForClient:' + element.name, marketMaker);
                   redis.hset(marketMaker +':MarketMakerPriorityForClient', element.name, element.priority.priority)
           });

       res.end();        
    });

http.createServer(app).listen(app.get('port'), function(){
  console.log('Express server listening on port ' + app.get('port'));
});