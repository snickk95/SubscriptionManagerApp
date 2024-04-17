import requests, json, os, urllib3, re
from pprint import pprint

base_url = 'https://localhost:7220/'

def post(route, body):
    url = os.path.join(base_url, route)
    url = os.path.normcase(url).replace("\\", "/")
    response = requests.post(url, verify=False, data=body, headers={'Content-Type': 'application/json'})


    output = response.status_code
    if response.status_code == 200:
        output = response.json()
    else:
        print("I'm not going any further due to", response)
    return output


def post_user(first_name, last_name, email):

    body = {"firstName": first_name, "lastName": last_name, "email": email, "subs": []}

    response = post("user", json.dumps(body))
    return response

pprint(post_user("Peter", "Madziak", "pmadziak@conestogac.on.ca"))