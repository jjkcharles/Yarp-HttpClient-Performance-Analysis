A sample repo to compare performance of calling an API through HttpClient vs. doing a direct forwarding using YARP.

This repo has two parts to it, 

1. **MyProxy** - C# Web appliction, that hosts an API which is a wrapper on top of [RESTCountries](https://restcountries.com/) API.
2. **LoadTest** - A [k6](https://k6.io/) script to run load test on APIs exposed by MyProxy.

### MyProxy
A simple MinimalAPI C# project which exposes two endpoints
1. /WithYarp/{countryCode}
2. /WithOutYarp/{countryCode}

As the name implies, first one uses YARP to do a direct forwarding of calls to RESTCountries.com API, whereas the other one uses HTTPClient to make calls to RESTCountries.com API before serving the response.

To run the code, simply do a dotnet run
```bash
dotnet run
```

Leave this application running before you start the K6 script under LoadTest

### LoadTest
Load Test performed on MyProxy exposed APIs against diferent country codes.

Based on the TEST_TYPE environment variable passed (accepts two values - WithYARP or WithOutYARP), this script tests either the YARP or non-YARP version.

To run this using docker containers use the below commands,
**WithOutYARP**
```bash
cd loadtest
cat load_script.js | docker run --rm -i grafana/k6 run -e TEST_TYPE=WithOutYARP --duration 30s -
```

**WithYARP**
```bash
cd loadtest
cat load_script.js | docker run --rm -i grafana/k6 run -e TEST_TYPE=WithYARP --duration 30s -
```

By default, this will run the test for 1000 Virtual Users - you can modify this in load_script.js