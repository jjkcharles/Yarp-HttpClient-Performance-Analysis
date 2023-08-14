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

### Observations
While the expectation I had was to see better results when using YARP for this usecase, it doesn't seem to be the case. With both approaches being pretty close to one another - and non-YARP being slightly better. 

Below are the results:
#### 100 VUs:
##### 1. With YARP:
```bash
     data_received..................: 53 MB  1.7 MB/s
     data_sent......................: 1.6 MB 52 kB/s
     http_req_blocked...............: avg=312.49µs min=1.02µs   med=4.92µs   max=101.02ms p(90)=8.66µs   p(95)=10.64µs
     http_req_connecting............: avg=242.63µs min=0s       med=0s       max=84.58ms  p(90)=0s       p(95)=0s
     http_req_duration..............: avg=94.38ms  min=73.03ms  med=86.67ms  max=1.05s    p(90)=108.52ms p(95)=119.59ms
       { expected_response:true }...: avg=94.38ms  min=73.03ms  med=86.67ms  max=1.05s    p(90)=108.52ms p(95)=119.59ms
     http_req_failed................: 0.00%  ✓ 0          ✗ 15655
     http_req_receiving.............: avg=650.98µs min=27.61µs  med=164.54µs max=27.6ms   p(90)=1.9ms    p(95)=3.29ms
     http_req_sending...............: avg=30.06µs  min=7.03µs   med=21.02µs  max=8.69ms   p(90)=57.81µs  p(95)=70.15µs
     http_req_tls_handshaking.......: avg=0s       min=0s       med=0s       max=0s       p(90)=0s       p(95)=0s
     http_req_waiting...............: avg=93.7ms   min=72.82ms  med=86.2ms   max=1.05s    p(90)=106.84ms p(95)=118.3ms
     http_reqs......................: 15655  506.465223/s
     iteration_duration.............: avg=975.4ms  min=883.36ms med=949.06ms max=1.89s    p(90)=1.01s    p(95)=1.07s
     iterations.....................: 3131   101.293045/s
     vus............................: 100    min=100      max=100
     vus_max........................: 100    min=100      max=100
```

##### 2. Without YARP:
```bash
     data_received..................: 52 MB  1.7 MB/s
     data_sent......................: 1.7 MB 55 kB/s
     http_req_blocked...............: avg=223.61µs min=982ns    med=4.23µs   max=105.75ms p(90)=7.74µs   p(95)=9.46µs
     http_req_connecting............: avg=199.54µs min=0s       med=0s       max=105.67ms p(90)=0s       p(95)=0s
     http_req_duration..............: avg=90.97ms  min=70.24ms  med=83.57ms  max=821.32ms p(90)=100.76ms p(95)=117.06ms
       { expected_response:true }...: avg=90.97ms  min=70.24ms  med=83.57ms  max=821.32ms p(90)=100.76ms p(95)=117.06ms
     http_req_failed................: 0.00%  ✓ 0          ✗ 15920
     http_req_receiving.............: avg=303.12µs min=16.81µs  med=118.48µs max=7.22ms   p(90)=674.68µs p(95)=1.27ms
     http_req_sending...............: avg=24.25µs  min=6.69µs   med=17.64µs  max=922.73µs p(90)=51.03µs  p(95)=62.88µs
     http_req_tls_handshaking.......: avg=0s       min=0s       med=0s       max=0s       p(90)=0s       p(95)=0s
     http_req_waiting...............: avg=90.64ms  min=70.14ms  med=83.29ms  max=821.23ms p(90)=100.03ms p(95)=116.43ms
     http_reqs......................: 15920  514.632283/s
     iteration_duration.............: avg=958.19ms min=881.91ms med=929.35ms max=1.73s    p(90)=997.35ms p(95)=1.06s
     iterations.....................: 3184   102.926457/s
     vus............................: 100    min=100      max=100
     vus_max........................: 100    min=100      max=100
```

#### 1000 VUs:
##### 1. With YARP:
```bash
     data_received..................: 48 MB  807 kB/s
     data_sent......................: 1.6 MB 26 kB/s
     http_req_blocked...............: avg=21.94ms  min=912ns   med=4.74µs  max=917.04ms p(90)=12.13µs p(95)=279.66ms
     http_req_connecting............: avg=10.06ms  min=0s      med=0s      max=832.76ms p(90)=0s      p(95)=103.75ms
     http_req_duration..............: avg=2.07s    min=79.59ms med=1.72s   max=10.11s   p(90)=3.51s   p(95)=3.79s
       { expected_response:true }...: avg=1.97s    min=79.59ms med=1.68s   max=10.11s   p(90)=3.13s   p(95)=3.6s
     http_req_failed................: 6.24%  ✓ 955        ✗ 14328
     http_req_receiving.............: avg=6.94ms   min=20.13µs med=3.6ms   max=88.76ms  p(90)=18.82ms p(95)=26.86ms
     http_req_sending...............: avg=378.05µs min=7.47µs  med=22.67µs max=44.5ms   p(90)=67.52µs p(95)=109.95µs
     http_req_tls_handshaking.......: avg=0s       min=0s      med=0s      max=0s       p(90)=0s      p(95)=0s
     http_req_waiting...............: avg=2.06s    min=79.35ms med=1.71s   max=10.11s   p(90)=3.51s   p(95)=3.79s
     http_reqs......................: 15283  254.709333/s
     iteration_duration.............: avg=10.98s   min=5.11s   med=10.92s  max=20.01s   p(90)=13.6s   p(95)=14.4s
     iterations.....................: 3055   50.915201/s
     vus............................: 2      min=2        max=1000
     vus_max........................: 1000   min=1000     max=1000
```

##### 2. Without YARP:
```bash
     data_received..................: 62 MB  1.0 MB/s
     data_sent......................: 2.0 MB 34 kB/s
     http_req_blocked...............: avg=21.08ms  min=942ns   med=4.84µs  max=1.43s    p(90)=11.2µs  p(95)=105.11ms
     http_req_connecting............: avg=12.89ms  min=0s      med=0s      max=1.31s    p(90)=0s      p(95)=63.84ms
     http_req_duration..............: avg=1.63s    min=92.06ms med=1.28s   max=5.68s    p(90)=2.89s   p(95)=3.24s
       { expected_response:true }...: avg=1.54s    min=92.06ms med=1.2s    max=5.68s    p(90)=2.63s   p(95)=3.03s
     http_req_failed................: 5.89%  ✓ 1134       ✗ 18088
     http_req_receiving.............: avg=15.23ms  min=28.14µs med=5.57ms  max=166.47ms p(90)=42.17ms p(95)=61.54ms
     http_req_sending...............: avg=322.24µs min=7.17µs  med=23.08µs max=65.55ms  p(90)=64.87µs p(95)=111.56µs
     http_req_tls_handshaking.......: avg=0s       min=0s      med=0s      max=0s       p(90)=0s      p(95)=0s
     http_req_waiting...............: avg=1.61s    min=91.94ms med=1.27s   max=5.67s    p(90)=2.87s   p(95)=3.23s
     http_reqs......................: 19222  320.363748/s
     iteration_duration.............: avg=8.76s    min=5.16s   med=8.64s   max=16.19s   p(90)=11.15s  p(95)=11.89s
     iterations.....................: 3843   64.049416/s
     vus............................: 4      min=4        max=1000
     vus_max........................: 1000   min=1000     max=1000
```