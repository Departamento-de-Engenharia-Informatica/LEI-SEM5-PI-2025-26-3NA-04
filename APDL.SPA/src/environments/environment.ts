export const environment = {
  production: false,
  apiUrl: 'https://localhost:5001/api',
  auth0: {
    domain: 'apdl-operations.eu.auth0.com',
    clientId: '9moFd6KqbCZnZImZrmhwHW3CTq6k9Qn0',
    authorizationParams: {
      audience: 'https://localhost:5001/api',
      redirect_uri: window.location.origin
    }
  }
};