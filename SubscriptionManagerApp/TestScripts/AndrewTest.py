import requests
import random
from datetime import datetime, timedelta
import json

subs_api_url = 'https://localhost:7220/subs-api'

def get_api_info():
    resp = requests.get(subs_api_url, verify=False)
    result = resp.json()
    
    subs_url = result['links']['subs']['href']
    users_url = result['links']['user']['href']

    return {'subs_url': subs_url, 'users_url': users_url} 

def get_all_subs(subs_url):
    resp = requests.get(subs_url, verify=False)
    result = resp.json()
    return result

def handle_random_sub(subs_url):
    all_subs = get_all_subs(subs_url)
    sub_list = all_subs['subLst']  # Access the list of subscriptions
    random_sub = random.choice(sub_list)
    print(f'The randomly selected subscription is...\nSubscription: {random_sub["serviceName"]}\nPrice: {random_sub["price"]}')



api_info = get_api_info()

subs_url = api_info['subs_url']
users_url = api_info['users_url']

handle_random_sub(subs_url)