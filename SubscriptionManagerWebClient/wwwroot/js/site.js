$(document).ready(function () {
    var _errorMsg = $('#errorMsg');
    var _subsList = $('#subsList');
    var _subsItemsMsg = $('#subsItemsMsg');
    var _totalPriceMsg = $('#monthlySpending');
    var _newSubMsg = $('#newSubMsg');

    var _subsApiHomeUrl = 'https://localhost:7220/subs-api';
    var _subsUrl = null;
    var _userUrl = null;

    var _isUserLoggedIn = false;
    var _loginLink = $('#loginLink');
    var _loginModal = $('#loginModal').modal();

    var _loggedInUserId = 0;

    var setLoginState = function (isLoggedIn) {
        _isUserLoggedIn = isLoggedIn;
        if (isLoggedIn) {
            _loginLink.text('Logout');
        } else {
            _loginLink.text('Login');
        }
    };

    var loadBaseApiInfo = async function () {
        const response = await fetch(_subsApiHomeUrl, {
            mode: 'cors',
            headers: {
                'Accept': 'application/json'
            }
        });

        if (response.status === 200) {
            const apiHomeResult = await response.json();
            const links = apiHomeResult.links;

            _subsUrl = links['subs'].href.trim();
            _userUrl = links['user'].href.trim();
        } else {
            _errorMsg.text('Hmmm, there was a problem accessing the quotes.');
            _errorMsg.fadeOut(10000);
        }
    };

    var loadSubs = async function (id) {

        let newUrl = _subsUrl;

        if (id != 0) {
            newUrl = _subsUrl + '/' + id;
        }

        const response = await fetch(newUrl, {         // get the quotes from API:
            mode: 'cors',
            headers: {
                'Accept': 'application/json'
            }
        });

        if (response.status === 200) {
            const subsResult = await response.json();
            const subs = subsResult.subLst;

            console.log(subs);

            displaySubs(subs);
            displaySubsSelect(subs);
        } else {
            _subsItemsMsg.text('Hmmm, there was a problem accessing the subscriptions.');
            _subsItemsMsg.fadeOut(10000);
        }
    };

    function displaySubs(subs) {
        let totalCost = 0;

        if (subs.length === 0) {
            _subsItemsMsg.text('No subscriptions yet - use the form to add some!');
        }
        else {
            // clear existing list:
            _subsList.empty();

            for (let i = 0; i < subs.length; i++) {
                _subsList.append('<li><b>Subscription:</b> ' + subs[i].serviceName + '<br />' +
                    '<b>Price:</b> ' + subs[i].price + '</li><br />');
                totalCost = totalCost + subs[i].price;
            }

            _totalPriceMsg.text('Total monthly spending: ' + totalCost);
        }
    }

    function displaySubsSelect(subs) {

        var selectElement = document.getElementById('subSelect');

        selectElement.innerHTML = '';

        subs.forEach(function (sub) {
            var option = document.createElement('option');
            option.value = sub.subscriptionId;
            option.textContent = sub.serviceName; 
            selectElement.appendChild(option);
        });
    };

    $('#userSearchBtn').click(async function () {
        const userId = $('#userId').val();

        const newUrl = _subsUrl + '/' + userId;

        const response = await fetch(newUrl, {        // Get the subs from ID
            method: 'GET',
            headers: {
                'Accept': 'application/json'
            }
        });

        if (response.status === 200) {
            const subsResult = await response.json();
            const subs = subsResult.subLst;

            console.log(subs);

            displaySubs(subs);
        } else {
            _subsItemsMsg.text('Hmmm, there was a problem accessing the subscriptions.');
            _subsItemsMsg.fadeOut(10000);
        }
    });

    $('#addSubBtn').click(async function () {
        var newUrl = _subsUrl + '/' + 2;
        var subId = document.getElementById('subSelect').value;

 //       console.log(subId);

        let newSub = {
            subscriptionId: subId,
            serviceName: $('#subName').val(),
            price: $('#subPrice').val()
        };

        const response = await fetch(newUrl, {
            method: 'POST',
            mode: 'cors',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(newSub)
        });

        if (response.status === 200) {
            _newSubMsg.attr('class', 'text-success');
            _newSubMsg.text('Subscription was added successfully.');
        }
        else if (response.status === 400) {
            _errorMsg.attr('class', 'text-danger');
            _errorMsg.text('You have already added this subscription.');
        }
        else if (response.status === 404) {
            _errorMsg.attr('class', 'text-danger');
            _errorMsg.text('The subscription requested could not be found.');
        }
        else {
            _errorMsg.attr('class', 'text-danger');
            _errorMsg.text('Hmmm, there was a problem adding the new subscription.');
        }
        _newSubMsg.fadeOut(10000);
        _errorMsg.fadeOut(10000);
    });

    //$('#registerBtn').click(async function () {

    //    let newUser = {
    //        firstName: $('#registerFName'),
    //        lastName: $('#registerLName'),
    //        email: $('#registerEmail')
    //    };

    //    const response = await fetch(_userUrl, {
    //        method: 'POST',
    //        mode: 'cors',
    //        headers: {
    //            'Content-Type': 'application/json'
    //        },
    //        body: JSON.stringify(newUser)
    //    });

    //    if (response.status === 200) {
    //        window.location.href = '/Index.cshtml';
    //});

    $('#loginLink').click(function () {
        _loginModal.modal('show');
    });

    $('#loginBtn').click(async function () {
        let newUrl = _userUrl + '/' + $('#loginEmail').val();

        const response = await fetch(newUrl, {
            method: 'GET',
            headers: {
                'Accept': 'application/json'
            }
        });

        if (response.status === 200) {
            const user = await response.json();

            setLoginState(true);

            console.log(response.body);
            _errorMsg.attr('class', 'text-success');
            _errorMsg.text('You have successfully logged in.');
            _errorMsg.fadeOut(10000);

            _loggedInUserId = user.userId;

            run();
        } else {
            loadSubs(_loggedInUserId);
            _errorMsg.attr('class', 'text-danger');
            _errorMsg.text('Hmmm, there was a problem logging you in.');
            _errorMsg.fadeOut(10000);
        }


        //loginPromise.then((response) => {
        //    if (response.status === 200) {
        //        return response.json();
        //    } else {
        //        return Promise.reject(response);
        //    }
        //})
        //    .then((tokenInfo) => {
        //        _currentAccessToken = tokenInfo.token;

        //        _newTaskItemMsg.attr('class', 'text-success');
        //        _newTaskItemMsg.text('You are logged in');
        //        setLoginState(true);
        //        $('#password').val('');

        //        run();
        //        _newTaskItemMsg.fadeOut(10000);
        //    })
        //    .catch((response) => {
        //        console.log(`fetch API home page; resp code: ${response.status}`);

        //        _newTaskItemMsg.attr('class', 'text-danger');
        //        _newTaskItemMsg.text('Hmmm, there was a problem logging you in.');
        //        _newTaskItemMsg.fadeOut(10000);
        //    });
    });

    let run = function () {
        // then setup a timer load tasks every 1 sec:
        setInterval(function () {
            if (_isUserLoggedIn) {
                loadSubs(_loggedInUserId);
            }
        }, 1000);
    };

    loadBaseApiInfo().then(() => {
        loadSubs(_loggedInUserId)
    });

});
