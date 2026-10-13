import http from 'k6/http';
import { check, sleep } from 'k6';
import { Counter } from 'k6/metrics';
import auth from './utils/authUtils.js';

const successfulReservations = new Counter('successful_reservations');
const conflictResponses = new Counter('conflict_responses');
const unexpectedResponses = new Counter('unexpected_responses');

export const options = {
  scenarios: {
    concurrent_booking: {
      executor: 'per-vu-iterations',
      vus: 2,
      iterations: 1,
      maxDuration: '30s',
    },
  },

  thresholds: {
    successful_reservations: ['count==1'],
    conflict_responses: ['count==1'],
    unexpected_responses: ['count==0'],
  },
};

// This function runs for each virtual user
export default function () {
  const accessToken = auth();

  const baseUrl = __ENV.API_BASE_URL;

  const url = `${baseUrl}/api/workspaceCalendar/6ac3c4740608853bc9c535b0/booking`;

  const params = {
    headers: {
      Authorization: `Bearer ${accessToken}`,
      'Content-Type': 'application/json',
    },
    timeout: '10s',
  };

  const payload = JSON.stringify({
    startAt: "2026-09-07T16:00:00Z",
		endAt: "2026-09-07T18:00:00Z"
  });

  const response = http.post(url, payload, params);

  if (response.status === 409) {
    conflictResponses.add(1);
  } else if ([200, 201].includes(response.status)) {
    successfulReservations.add(1);
  } else {
    unexpectedResponses.add(1);
  }

  check(response, {
    'resposta é sucesso ou conflito esperado': (r) =>
      [200, 201, 409].includes(r.status),
  });

  // Simulate realistic user behavior with sleep
  sleep(0.5);
}