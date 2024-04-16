$(document).ready(function () {
    var _errorMsg = $('#errorMsg');
    var _subsList = $('#subsList');
    var _subsItemsMsg = $('#subsItemsMsg');
    var _totalPriceMsg = $('#monthlySpending');
    var _newSubMsg = $('#newSubMsg');

    var _subsApiHomeUrl = 'https://localhost:7220/subs-api';
    var _subsUrl = null;
    var _userUrl = null;

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

    var loadSubs = async function () {
        const response = await fetch(_subsUrl, {         // get the quotes from API:
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

    var displaySubsSelect = function (subs) {

        var selectElement = document.getElementById('subSelect');

        selectElement.innerHTML = '';

        subs.forEach(function (sub) {
            var option = document.createElement('option');
            option.value = sub.subscriptionId; // You need to replace 'value' with the actual property name of your subscription object
            option.textContent = sub.serviceName; // You need to replace 'text' with the actual property name of your subscription object
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
        var newUrl = _subsUrl + '/' + 1;
        var subId = document.getElementById('subSelect').value;

        console.log(subId);

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

        if (response.status === 201) {
            _newSubMsg.attr('class', 'text-success');
            _newSubMsg.text('Quote was added successfully.');
        } else {
            _errorMsg.attr('class', 'text-danger');
            _errorMsg.text('Hmmm, there was a problem adding the new subscription.');
        }
        _newSubMsg.fadeOut(10000);
        _errorMsg.fadeOut(10000);
    });

    loadBaseApiInfo().then(() => {
        loadSubs()
    });

});
