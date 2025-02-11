import React, { useReducer, FC } from 'react';
import { BrowserRouter } from 'react-router-dom';
import Layout from './layout/layout';
import './App.css';
import { DarkTheme } from './ux/theme';
import { AppContext, ApplicationState, getDefaultState } from './models/applicationState';
import appReducer from './reducers';
import { TodoContext } from './components/todoContext';
import { initializeIcons } from '@fluentui/react/lib/Icons';
import { ThemeProvider } from '@fluentui/react';
import { Auth0Provider } from '@auth0/auth0-react';

export const App: FC = () => {
  const defaultState: ApplicationState = getDefaultState();
  const [applicationState, dispatch] = useReducer(appReducer, defaultState);
  const initialContext: AppContext = { state: applicationState, dispatch: dispatch }
  const domain = process.env.REACT_APP_AUTH0_DOMAIN??"";
  const clientId = process.env.REACT_APP_AUTH0_CLIENT_ID??"";
  const api_audience = process.env.REACT_APP_AUTH0_AUDIENCE ?? "";
  
  initializeIcons();

  return (
  <Auth0Provider
      domain={domain}
      clientId={clientId} 
      authorizationParams={{
        redirect_uri: window.location.origin,
        audience: api_audience
      }}
    >
    <ThemeProvider applyTo="body" theme={DarkTheme}>
      <TodoContext.Provider value={initialContext}>
        <BrowserRouter>
            <Layout />
        </BrowserRouter>
      </TodoContext.Provider>
    </ThemeProvider>
  </Auth0Provider>
  );
};
