
var app = angular.module('dmsApp', ['smart-table', 'ngRoute', 'ui.bootstrap','dndLists','ui.select2']);

app.directive('compileTemplate', function ($compile, $parse) {
    return {
        link: function (scope, element, attr) {
            var parsed = $parse(attr.ngBindHtml);
            function getStringValue() { return (parsed(scope) || '').toString(); }

            //Recompile if the template changes
            scope.$watch(getStringValue, function () {
                $compile(element, null, -9999)(scope);  //The -9999 makes it skip directives so that we do not recompile ourselves
            });
        }
    }
});
//app.config(function ($locationProvider) {
//    $locationProvider.html5Mode({
//        enabled: true
//    });
//});

  