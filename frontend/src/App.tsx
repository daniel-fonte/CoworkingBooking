import { ReactKeycloakProvider } from '@react-keycloak/web'
import keycloak from './keycloak'
import Routes from './routes';

function App() {
  return ( 
    <ReactKeycloakProvider 
      authClient={keycloak}
      initOptions={{
        onLoad: "check-sso",
        pkceMethod: "S256",
      }}
    >
      <Routes />
    </ReactKeycloakProvider>
  )
}

export default App
