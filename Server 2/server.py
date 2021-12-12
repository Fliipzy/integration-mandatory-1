from __future__ import print_function
from bottle import post, run, request, response
from dicttoxml import dicttoxml
import json

# open appsettings json file
appsettings = open("../appsettings.json")

# convert appsettings json object to dictionary
serverinfo = json.load(appsettings)["server2Settings"]

@post("/" + serverinfo["endpoint"])
def get_data():
    response.headers["Content-Type"] = "xml/application"
    jsondata = request.json
    xmldata = dicttoxml(jsondata, attr_type=False)
    return xmldata

run(host=serverinfo["address"], port=serverinfo["port"])