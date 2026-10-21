import Keycloak from 'keycloak-js';

// Setup Keycloak instance as needed
// Pass initialization options as required or leave blank to load from 'keycloak.json'
const keycloak = new Keycloak({
  url: 'http://localhost:7080',
  realm: 'coworking-booking',
  clientId: 'coworking-booking-frontend',
});

export default keycloak