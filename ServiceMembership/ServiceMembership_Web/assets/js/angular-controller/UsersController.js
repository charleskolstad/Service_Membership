angular.module("Membership", ["ngRoute"]).config(function ($routeProvider) {
    $routeProvider.when("/Users", {
        templateUrl: "/Admin/AllUsers/"
    });
})

angular.module("Membership").controller("MembershipCtrl", function ($scope, $http) {
    $scope.data = {};
    $scope.UserInfo = {};
    $scope.MembershipProfile = {};
    $scope.UserName;
        
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

    $scope.getUser = function (userName) {

        $http({
            method: "get",
            url: "http://localhost:57259/Admin/UserGetByID?userName=" + userName,
        }).then(function (response) {
            document.getElementById('modalTitle').innerHTML = 'Update user - ' + userName;
            $scope.data.UserInfo = response.data;
            $scope.UserInfo.UName = response.data.UName;
            $scope.UserInfo.UserInfoID = response.data.UserInfoID;
            $scope.UserInfo.LastName = response.data.LastName;
            $scope.UserInfo.FirstName = response.data.FirstName;
            $scope.UserInfo.PhoneNumber = response.data.PhoneNumber;
            $scope.UserInfo.Email = response.data.Email;
            for (var i = 0; i < $scope.data.Profiles.length; i++)
            {                
                var obj = findObjectByKey(
                    response.data.UserProfiles,
                    'ProfileID',
                    $scope.data.Profiles[i].ProfileID);

                $scope.data.Profiles[i].Active = obj.Active;
            }
            angular.element('#reservationModal').modal('show');
        }, function () {

        });
    }

    $scope.CancelChanges = function ()
    {
        document.getElementById('modalTitle').innerHTML = 'Create New User';
        $scope.UserInfo.UName = '';
        $scope.UserInfo.UserInfoID = '';
        $scope.UserInfo.LastName = '';
        $scope.UserInfo.FirstName = '';
        $scope.UserInfo.PhoneNumber = '';
        $scope.UserInfo.Email = '';
        for (var i = 0; i < $scope.data.Profiles.length; i++)
        {
            $scope.data.Profiles[i].Active = false;
        }
    }

    $scope.deleteModal = function (userName) {
        $scope.UserName = userName;
        document.getElementById('DeleteMessage').innerHTML = 'Are you sure you want to delete user - ' + userName;
        angular.element('#deleteModal').modal('show');
    }

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

    $scope.deleteUser = function ()
    {
        $http({
            method: "get",
            url: "http://localhost:57259/Admin/DeleteUser?userName=" + $scope.UserName,
        }).then(function (response) {
            alert(response.data);
        });
    }

    function findObjectByKey(array, key, value) {
        for (var i = 0; i < array.length; i++) {
            if (array[i][key] === value) {
                return array[i];
            }
        }
        return null;
    }
});