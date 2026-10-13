import http from 'k6/http';

const auth = () => {
    const baseUrl = __ENV.AUTH_BASE_URL;

    const headers = { 'Content-Type': 'application/x-www-form-urlencoded' };

    const payload = JSON.stringify({
        grant_type: "password",
        client_id: "coworking-booking-api",
        username: "admin",
        password: "admin123"
    });

    const response = http.post(`${baseUrl}/realms/coworking-booking/protocol/openid-connect/token`, payload, { headers });

    const body = response.json();

    return body.access_token;
}

export default auth;