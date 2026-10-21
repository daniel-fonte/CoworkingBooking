import { ReactKeycloakProvider } from '@react-keycloak/web'
import { BrowserRouter, Route, Routes } from 'react-router-dom'
import keycloak from './keycloak'
import Login from './components/Login'

const eventLogger = (event: unknown, error: unknown) => {
  console.log('onKeycloakEvent', event, error)
}

const tokenLogger = (tokens: unknown) => {
  console.log('onKeycloakTokens', tokens)
}

// Wrap everything inside KeycloakProvider
function App() {
  return ( 
    <ReactKeycloakProvider 
      authClient={keycloak}
      onEvent={eventLogger}
      onTokens={tokenLogger}
    >
        <BrowserRouter>
          <Routes>
            <Route
              path="/"
              element={
                <Login />
              }
            />
          </Routes>
        </BrowserRouter>
    </ReactKeycloakProvider>
  )
}

export default App
