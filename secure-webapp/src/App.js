import NavBar from './components/NavBar';
import SignIn from './pages/SignIn';
import { UserProvider } from './containers/UserContext';
import { Route, Routes } from 'react-router-dom';
import React from 'react';
import PrivateRoute from './containers/PrivateRoute';
import Home from './pages/Home';
import SignUp from './pages/SignUp';
import UsersTable from './pages/UsersTable';
import EncryptFile from './pages/EnryptFile';

function App() {
  return (
    <UserProvider>
      <NavBar />
      <React.Suspense fallback={<div>Loading...</div>}>
        <Routes>
          <Route path="/" element={<Home />} />
          <Route
            path="/users-table"
            element={
              <PrivateRoute>
                <UsersTable />
              </PrivateRoute>
            }
          />
          <Route
            path="/encrypt-file"
            element={
              <PrivateRoute>
                <EncryptFile />
              </PrivateRoute>
            }
          />
          <Route path="/signin" element={<SignIn />} />
          <Route path="/signup" element={<SignUp />} />
          <Route path="*" element={<Home />} />
        </Routes>
      </React.Suspense>
    </UserProvider>
  );
}

export default App;
