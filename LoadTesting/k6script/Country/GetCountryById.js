import http from 'k6/http';
import { check, sleep } from 'k6';

export let options = {
    stages: [
        { duration: '1m', target: 10 },  // Ramp up to 10 users
        { duration: '3m', target: 50 }, // Hold at 50 users
        { duration: '1m', target: 0 },   // Ramp down
    ],
};

const BASE_URL = 'http://host.docker.internal:5000/api/country';

export default function () {
    const countryId = 1; // Replace with a valid country ID in your database

    // Test the `GetCountryById` endpoint
    let response = http.get(`${BASE_URL}/${countryId}`);
    check(response, {
        'GetCountryById: status is 200': (r) => r.status === 200,
        'GetCountryById: response time < 500ms': (r) => r.timings.duration < 500,
        'GetCountryById: has country name': (r) => JSON.parse(r.body).name !== undefined,
    });

    sleep(1); // Simulate a short delay between requests
}
