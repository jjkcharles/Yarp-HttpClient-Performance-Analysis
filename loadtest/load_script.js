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
    http.get('https://host.docker.internal:7254/withyarp/can');
    http.get('https://host.docker.internal:7254/withyarp/mex');
    http.get('https://host.docker.internal:7254/withyarp/esp');
    http.get('https://host.docker.internal:7254/withyarp/cam');
    http.get('https://host.docker.internal:7254/withyarp/per');
}

function WithoutYarp() {
    http.get('https://host.docker.internal:7254/withoutyarp/can');
    http.get('https://host.docker.internal:7254/withoutyarp/mex');  
    http.get('https://host.docker.internal:7254/withoutyarp/esp');  
    http.get('https://host.docker.internal:7254/withoutyarp/cam');  
    http.get('https://host.docker.internal:7254/withoutyarp/per');
}