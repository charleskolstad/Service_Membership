angular.module("Membership", ["ngRoute"]).config(function ($routeProvider) {
    $routeProvider.when("/Users", {
        templateUrl: "/Admin/AllUsers/"
    });
})

angular.module("Membership").controller("MembershipCtrl", function ($scope, $http) {
    $scope.data = {};
    $scope.UserInfo = {};
    $scope.MembershipProfile = {};
        
    $http({
        method: "get",
        url: "http://localhost:57259/Admin/UsersGetAll"
    }).then(function (response) {
        $scope.data.Users = response.data;
    }, function () {

    });
    

    $http({
        method: "get",
        url: "http://localhost:57259/Admin/ProfilesGetAll"
    }).then(function (response) {
        $scope.data.Profiles = response.data;
    }, function () {

    });

    $scope.saveUser = function () {
        $scope.UserInfo.UserProfiles = [];

        for (var j = 0; j < $scope.data.Profiles.length; j++)
        {
            var obj = {
                UserName: $scope.data.Profiles[j].ProfileName,
                ProfileID: $scope.data.Profiles[j].ProfileID,
                Active: $scope.data.Profiles[j].Active
            }
            $scope.UserInfo.UserProfiles.push(obj);
        }
        
        $http({
            method: "post",
            url: "http://localhost:57259/Admin/CreateUser",
            datatype: "json",
            data: JSON.stringify($scope.UserInfo)
        }).then(function (response) {
            if (response == "") {
                angular.element('#reservationModal').modal('hide');
                alert('User ' + $scope.UName + ' created successfully!');
                $scope.UName = "";
                $scope.Email = "";
                $scope.FirstName = "";
                $scope.LastName = "";
                $scope.PhoneNumber = "";
            }
            else {
                document.getElementById("lblError").innerHTML = response.data;
                document.getElementById("lblError").style.display = 'block';
            }
        })
    }
});