import requests, json, os, urllib3, re
from pprint import pprint

urllib3.disable_warnings()

# API base URL
base_url = 'https://localhost:7220/'

def get(route):
    url = os.path.join(base_url, route)
    url = os.path.normcase(url).replace("\\", "/")

    response = requests.get(url, verify=False)

    output = response.status_code
    if response.status_code == 200:
        output = response.json()
    else:
        print("I'm not going any further due to", response)
    return output

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


my_response = get("user/sherlock@example.com")



def get_user(email):
    response = get(os.path.join("user", email))
    return response
    
def get_subs():
    response = get("subs")
    return response

def get_subs_id(user_id):
    response = get(os.path.join("subs", user_id))
    return response

def post_user(first_name, last_name, email):

    body = {"firstName": first_name, "lastName": last_name, "email": email, "subs": []}

    response = post("user", json.dumps(body))
    return response

def post_sub_id(user_id, sub_id):
    body = {"subscriptionId": sub_id }
    response = post(os.path.join("subs", str(user_id)), json.dumps(body))
    return response


# Menu options
user_options = [
    "Get User",
    "Get Subscriptions",
    "Get User's Subscriptions by ID",
    "Add User",
    "Set User Subsctiption ID",
    "Exit"
]
# Just for Space
for i in range(0,10):
    print()

print("Pick an option:")

is_looping = True
# Keeps looping until we kill it with 5
while is_looping:
    i = 0
    for option in user_options:
        print(i, option, sep=": ")
        i += 1
    user_input = input("")
    
    if user_input == "":
        print("Please input something.")
        continue
    
    # if number is not in our accepted range
    try:
        x = int(user_input)
        if not (x >= 0 and x < len(user_options)):
            raise IndexError
        
    except:
        print("Input is not a valid")

    else:
        user_input = int(user_input)

        match user_input:
            # We terminated the program
            case 5:
                print("Thank you come again")
                is_looping = False

            case 0:
                # we Are searching for a user by his email
                print("Please Input an Email")
                email = input()
                res = get_user(email)
                pprint(res)

            case 1:
                # we want a list of all the subscriptions
                pprint(get_subs())
                

            case 2:
                # We
                print("Please Input a User Id")
                user_id = input()
                pprint(get_subs_id(user_id))

            case 3:
                print("Please enter a first name")
                first_name = input()
                print("Please enter a Last name")
                last_name = input()
                print("Please enter an email")
                email = input()
                pprint(post_user(first_name,last_name,email))


            case 4:
                print("Please enter a User to add a subscription to")
                user_id = input()
                print("Please enter a Subscription to add")
                sub_id = input()
                pprint(post_sub_id(user_id,sub_id))    
