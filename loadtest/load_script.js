import http from 'k6/http';
import { sleep } from 'k6';

export const options = {
    vus: 1000,
    insecureSkipTLSVerify: true,
};

export default function () {
    if(__ENV.TEST_TYPE == 'WithYARP')
    {
        //console.log("Executing WithYARP Tests");
        WithYarp();
    }
    else if(__ENV.TEST_TYPE == 'WithOutYARP') {
        //console.log("Executing WithOutYARP Tests");
        WithoutYarp();
    }    
    sleep(0.5);
}

function WithYarp() {
    http.get('http://host.docker.internal:5272/withyarp/can');
    http.get('http://host.docker.internal:5272/withyarp/mex');
    http.get('http://host.docker.internal:5272/withyarp/esp');
    http.get('http://host.docker.internal:5272/withyarp/cam');
    http.get('http://host.docker.internal:5272/withyarp/per');
}

function WithoutYarp() {
    http.get('http://host.docker.internal:5272/withoutyarp/can');
    http.get('http://host.docker.internal:5272/withoutyarp/mex');  
    http.get('http://host.docker.internal:5272/withoutyarp/esp');  
    http.get('http://host.docker.internal:5272/withoutyarp/cam');  
    http.get('http://host.docker.internal:5272/withoutyarp/per');
}