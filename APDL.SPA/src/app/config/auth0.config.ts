export const auth0Config = {
  domain: 'apdl-operations.eu.auth0.com',
  clientId: '9moFd6KqbCZnZImZrmhwHW3CTq6k9Qn0',
  authorizationParams: {
    redirect_uri: window.location.origin + '/home',
    audience: 'https://localhost:5001/api',
    scope: 'openid profile email'
  },
  
  useRefreshTokens: true,
  cacheLocation: 'localstorage' as const,
  
  httpInterceptor: {
    allowedList: [
      'https://localhost:5001/api/*',
    ],
  },
};