const express = require("express");
const app = express();
const bodyParser = require("body-parser");
const fetch = require("node-fetch");

//xml to csv packages
const xml2js = require("xml2js");
const json2csv = require("json2csv");

// express config
app.use(bodyParser.text());

const csvtojson = require("csvtojson/v2");

// get appsettings.json
const { server1Settings: server1, server2Settings: server2 } = require("../appsettings.json");

app.post("/" + server1.endpoint, (req, res) => {
    res.set("Content-Type", "text/plain");

    csvtojson().fromString(req.body)
        .then(async (jsonData) => {
            // before responding, get the XML data from server 2
            // then convert the XML to CSV and send that back to the program.
            let xmlData = await getXmlDataFromServer2(jsonData);
            convertXMLToCSV(xmlData, function(csvData) {
                return res.send(csvData);
            });
        });
});

async function getXmlDataFromServer2(jsonData) {
    const response = await fetch("http://" + server2.address + ":" + server2.port + "/" +  server2.endpoint, {
            method: "POST",
            headers: {"Content-Type": "application/json"},
            body:  JSON.stringify(jsonData)
        });
    
    return await response.text();
}

function convertXMLToCSV(xmlData, callback) {
    xml2js.parseString(xmlData, { explicitArray: false, explicitRoot: false }, (error, jsonData) => {
        if (error) {
            console.log(error);
            return null;
        }
        const csvData = json2csv.parse(jsonData.item);
        callback(csvData);
    });
}

app.listen(server1.port, (err) => {
    if (err) {
        console.log("Server cannot listen...");
        return;
    }
    console.log("Server started on port", server1.port);
});