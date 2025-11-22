export const environment = {
  production: true,
  apiUrl: 'https://10.9.11.75/api',
  auth0: {
    domain: 'apdl-operations.eu.auth0.com',
    clientId: '9moFd6KqbCZnZImZrmhwHW3CTq6k9Qn0',
    authorizationParams: {
      audience: 'https://localhost:5001/api',
      redirect_uri: window.location.origin,
      scope: 'openid profile email offline_access'
    }
  }
};