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

    var _registerLink = $('#registerLink');
    var _registerModal = $('#registerModal').modal();

    var _loggedInUserId = 0;

    var setLoginState = function (isLoggedIn) {
        _isUserLoggedIn = isLoggedIn;
        if (isLoggedIn) {
            _loginLink.text('Logout');
            _registerLink.text('');
        } else {
            _loginLink.text('Login');
            _registerLink.text('Register');
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
            _errorMsg.fadeOut(5000);
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
            loadSubsSelect();
        } else {
            _subsItemsMsg.text('Hmmm, there was a problem accessing the subscriptions.');
            _subsItemsMsg.fadeOut(5000);
        }
    };

    var loadSubsSelect = async function () {

        const response = await fetch(_subsUrl, {         // get the quotes from API:
            mode: 'cors',
            headers: {
                'Accept': 'application/json'
            }
        });

        if (response.status === 200) {
            const subsResult = await response.json();
            const subs = subsResult.subLst;

            displaySubsSelect(subs);
        } else {
            _subsItemsMsg.text('Hmmm, there was a problem loading the subscriptions into the select box.');
            _subsItemsMsg.fadeOut(5000);
        }
    };

    function displaySubs(subs) {
        let totalCost = 0;

        if (subs.length === 0) {
            _subsList.empty();
            _subsItemsMsg.text('No subscriptions yet - use the form to add some!');
            _subsItemsMsg.fadeOut(5000);

            totalCost = 0;
        }
        else {
            // clear existing list:
            _subsList.empty();

            for (let i = 0; i < subs.length; i++) {
                _subsList.append('<li><b>Subscription:</b> ' + subs[i].serviceName + '<br />' +
                    '<b>Price:</b> ' + subs[i].price + '<br /><button type="button" class="btn btn-primary deleteBtn" data-sub-id="' +
                    subs[i].subscriptionId + '">Delete</button>' + '</li><br />');
                totalCost = totalCost + subs[i].price;
            }
            $('.deleteBtn').click(handleDeleteButtonClick);

        }
        _totalPriceMsg.text('Total monthly spending: ' + totalCost);
    }

    async function handleDeleteButtonClick() {
        var loggedInUserId = getCookie('_loggedInId');

        let subId = $(this).data('sub-id');
        let newUrl = _subsUrl + '/' + loggedInUserId + '?subId=' + subId;

        const response = await fetch(newUrl, {
            method: 'Delete',
            mode: 'cors',
            headers: {
                'Accept': 'application/json'
            }
        });

        if (response.status === 200) {
            _subsItemsMsg.attr('class', 'text-success');
            _subsItemsMsg.text('Tag successfully deleted.');
            _subsItemsMsg.fadeOut(5000);

        } else {
            _errorMsg.attr('class', 'text-danger');
            _errorMsg.text('Hmmm, there was a problem deleting the tag.');
            _errorMsg.fadeOut(5000);
        }
        loadSubs(loggedInUserId);
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

    $('#createSubBtn').click(async function () {

        let newSub = {
            serviceName: $('#newSubName').val(),
            price: $('#newSubPrice').val()
        };

        const response = await fetch(_subsUrl, {
            method: 'POST',
            mode: 'cors',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(newSub)
        });

        if (response.status === 200) {
            _newSubMsg.attr('class', 'text-success');
            _newSubMsg.text('Subscription was created successfully.');

            var loggedInUserId = getCookie('_loggedInId');
            loadSubs(loggedInUserId);
        } else {
            _errorMsg.attr('class', 'text-danger');
            _errorMsg.text('Hmmm, there was a problem creating the new subscription.');
        }
        _newSubMsg.fadeOut(5000);
        _errorMsg.fadeOut(5000);

        $('#newSubName').val('');
        $('#newSubPrice').val('')
    });

    $('#addSubBtn').click(async function () {
        var loggedInUserId = getCookie('_loggedInId');
        var newUrl = _subsUrl + '/' + loggedInUserId;
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
        _newSubMsg.fadeOut(5000);
        _errorMsg.fadeOut(5000);

        var loggedInUserId = getCookie('_loggedInId');
        loadSubs(loggedInUserId);
    });

    $('#loginLink').click(function () {
        var loggedInUserId = getCookie('_loggedInId');

        if (loggedInUserId != 0) {
            setLoginState(false);
            setCookie('_loggedInId', 0);
            loadSubs(0);
            $('#addSubBtn').prop('disabled', true);
        }
        else {
            _loginModal.modal('show');
        }
    });

    $('#loginBtn').click(async function () {
        
        newUrl = _userUrl + '/' + $('#loginEmail').val();

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
            _errorMsg.fadeOut(5000);

            //            _loggedInUserId = user.userId;
            setCookie('_loggedInId', user.userId);

            loadSubs(user.userId);
            $('#addSubBtn').prop('disabled', false);
            _subsItemsMsg.attr('class', 'text-success');
            _subsItemsMsg.text('You have successfully logged in!');
            _subsItemsMsg.fadeOut(5000);
        } else {
            var loggedInUserId = getCookie('_loggedInId');
            loadSubs(loggedInUserId);
            _errorMsg.attr('class', 'text-danger');
            _errorMsg.text('Hmmm, there was a problem logging you in.');
            _errorMsg.fadeOut(5000);
        }

    });

    $('#registerLink').click(function () {
        _registerModal.modal('show');
    });

    $('#registerBtn').click(async function () {
        let fName = $('#registerFName').val();
        let lName = $('#registerLName').val();
        let regemail = $('#registerEmail').val();

        if (!fName || !lName || !regemail) {
            _subsItemsMsg.attr('class', 'text-danger');
            _subsItemsMsg.text('Please fill in all fields.');
            _subsItemsMsg.fadeOut(5000);
            return;
        }

        let newUser = {
            firstName: fName,
            lastName: lName,
            email: regemail
        };

        const response = await fetch(_userUrl, {
            method: 'POST',
            mode: 'cors',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(newUser)
        });

        if (response.status === 200) {
            const user = await response.json();

            setLoginState(true);

            console.log(response.body);

            //           _loggedInUserId = user.userId;
            setCookie('_loggedInId', user.userId);

            loadSubs(user.userId);
            $('#addSubBtn').prop('disabled', false);
            _subsItemsMsg.attr('class', 'text-success');
            _subsItemsMsg.text('You have successfully registered!');
            _subsItemsMsg.fadeOut(5000);
        } else {
            var loggedInUserId = getCookie('_loggedInId');
            loadSubs(loggedInUserId);
            _errorMsg.attr('class', 'text-danger');
            _errorMsg.text('Hmmm, there was a problem with getting you registered');
            _errorMsg.fadeOut(5000);
        }

    });

    function setCookie(name, value) {
        document.cookie = name + "=" + encodeURIComponent(value);
    }

    function getCookie(name) {
        const cookieString = document.cookie;
        const cookieArray = cookieString.split('; ');

        for (let cookie of cookieArray) {
            const [cookieName, cookieValue] = cookie.split('=');
            if (cookieName === name) {
                return decodeURIComponent(cookieValue);
            }
        }

        return null;
    }

    loadBaseApiInfo().then(() => {
        var loggedInUserId = getCookie('_loggedInId');
        if (loggedInUserId == null) {
            setCookie('_loggedInId', 0);
            loggedInUserId = 0;
            $('#addSubBtn').prop('disabled', true);
            $('.deleteBtn').prop('disabled', true);
        } else if (loggedInUserId == 0) {
            $('#addSubBtn').prop('disabled', true);
            $('.deleteBtn').prop('disabled', true);
        } else {
            setLoginState(true);
        }
        loadSubs(loggedInUserId)
    });

});
