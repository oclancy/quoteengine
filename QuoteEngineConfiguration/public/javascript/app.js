
// declare a module
var myApp = angular.module('myApp', ['ui.grid', 'ui.grid.edit', 'ui.grid.selection']);


myApp.controller('brokerConfigController', ['$scope', "$http", function($scope, $http) {
  $scope.title = 'Configure Broker';

    $scope.clients = [
        {name:"Client1"},
        {name:"Client2"},
        {name:"Client3"},
    ];

  $scope.broker = {
      name:"broker1", 
      symbols:[], 
      priorities: [
          { priority:10, buyBp: 0, sellBp: 0, tradeInAuction:false, tradeWorseThanTouch:false },
          { priority:20, buyBp: 0, sellBp: 0, tradeInAuction:false, tradeWorseThanTouch:false },
          { priority:30, buyBp: 0, sellBp: 0, tradeInAuction:false, tradeWorseThanTouch:false }
         ]};

  $scope.broker.clients = [
          { "name": $scope.clients[0].name, priority: $scope.broker.priorities[0] },
          { "name": $scope.clients[1].name, priority: $scope.broker.priorities[1] },
          { "name": $scope.clients[2].name, priority: $scope.broker.priorities[2] }
      ];

  $scope.brokerConfig={
      enableRowSelection: true,
      enableSelectAll: false,
      multiSelect: false,

      onRegisterApi: function(gridApi){
          //set gridApi on scope
          $scope.gridApi = gridApi;
          gridApi.selection.on.rowSelectionChanged($scope,function(row){
            
      })},

      columnDefs:[
          {name:"symbol", field:"symbol", type:'string', enableCellEdit:'true'},
          {name:"isin", field:"isin", type:'string', enableCellEdit:'true'},
          {name:"mic", field:"mic", type:'string', enableCellEdit:'true'},
          
          {name:"buy qty", field:"buyVol", type:'number', enableCellEdit:'true'},
          {name:"sell qty", field:"sellVol", type:'number', enableCellEdit:'true'},

          {name:"Auto Fill", field:"autoFill", type:'boolean', enableCellEdit:'true', cellTemplate: '<input type="checkbox" ng-model="row.entity.autoFill"/>' },
          ],
      data:$scope.broker.symbols
      };


      $scope.addData = function() {
        var n = $scope.brokerConfig.data.length + 1;

        var newRow = {
                    "symbol": "",
                    "isin": "",
                    "mic": "",
                    "buyVol": 0,
                    "sellVol": 0,
                    "autoFill": true
                  };

        $scope.brokerConfig.data.push(newRow);
      };

      $scope.removeLine = function() {

      };

      $scope.save = function() {
          $http
          .post("/"+$scope.broker.name, {symbols:$scope.brokerConfig.data,
                                         priorities:$scope.priorityConfig.data,
                                         clients:$scope.clientConfig.data})
          .success(function(){
                $scope.broker.symbols = $scope.brokerConfig.data;
                $scope.broker.priorities = $scope.prioritiesConfig.data,
                $scope.broker.clients = $scope.clientsConfig.data
              });
      };

      $scope.priorityConfig={
        enableRowSelection: true,
        enableSelectAll: false,
        multiSelect: false,
        onRegisterApi: function( gridApi ) { 
            $scope.gridApi = gridApi;
            var cellTemplate = 'ui-grid/selectionRowHeader';   // you could use your own template here
            $scope.gridApi.core.addRowHeaderColumn( { name: 'priority', displayName: '', width: 30, cellTemplate: cellTemplate} );
        },

        columnDefs:[
          
            {name:"buy bp", field:"buyBp", type:'number', enableCellEdit:'true'},
            {name:"sell bp", field:"sellBp", type:'number', enableCellEdit:'true'},

            {name:"Trade In Auction", field:"tradeInAuction", type:'boolean', enableCellEdit:'true', cellTemplate: '<input type="checkbox" ng-model="row.entity.tradeInAuction"/>' },
            {name:"Trade Worse Than Touch", field:"tradeWorseThanTouch", type:'boolean', enableCellEdit:'true', cellTemplate: '<input type="checkbox" ng-model="row.entity.tradeWorseThanTouch"/>'},
            ],
        data: $scope.broker.priorities
        };

      $scope.clientConfig={
          enableRowSelection: true,
          enableSelectAll: false,
          multiSelect: false,
            onRegisterApi: function( gridApi ) { 
                $scope.gridApi = gridApi;
                var cellTemplate = 'ui-grid/selectionRowHeader';   // you could use your own template here
                $scope.gridApi.core.addRowHeaderColumn( { name: 'priority', displayName: '', width: 30, cellTemplate: cellTemplate} );
            },

            columnDefs:[
          
                {name:"name", field:"name"},
                {name:"priority", field:"priority.priority"}
                ],
            data: $scope.broker.clients
        };

    $http.get("/data/"+$scope.broker.name)
         .success(function(data){
                //$scope.brokerConfig.data = data || [];
             });
}]);