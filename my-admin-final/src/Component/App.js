import React, { useEffect, useState } from 'react';
import { BrowserRouter as Router, Route, Routes, Navigate} from 'react-router-dom';
import Login from '../Pages/Login/Login';
import AdminDashboard from '../Pages/Dashboard/AdminDashboard';
import Product from '../Pages/Product/Product';
import ProductEdit from '../Pages/Product/ProductEdit';
import ProductAdd from '../Pages/Product/ProductAdd';
import Invoice from '../Pages/Invoice/Invoice';
import User from '../Pages/User/User';
import UserAdd from '../Pages/User/UserAdd';

const App = () => {
    const [loggedIn, setLoggedIn] = useState(false);

    const handleLogin = () => { 
      setLoggedIn(true);
    }

  const handleLogout = () => {
      setLoggedIn(false);
    }

  useEffect(() => {
    const token = localStorage.getItem('jwt');
    if (token) {
      setLoggedIn(true);
    }
  }, []);

  return (
    <Router>
      <Routes>
        <Route
          path="/login"
          element={<Login setLoggedIn={handleLogin} />}
        />
        <Route
          path="/"
          element={loggedIn ? <AdminDashboard onLogout={handleLogout} /> : <Navigate to="/login" />}
        />
        <Route
          path="/users"
          element={loggedIn ? <User onLogout={handleLogout} /> : <Navigate to="/login" />}
        />
        <Route
          path="/users/register-admin"
          element={loggedIn ? <UserAdd onLogout={handleLogout} /> : <Navigate to="/login" />}
        />
        <Route
          path="/products"
          element={loggedIn ? <Product onLogout={handleLogout} /> : <Navigate to="/login" />}
        />
        <Route
          path="/products/edit/:id"
          element={loggedIn ? <ProductEdit onLogout={handleLogout} /> : <Navigate to="/login" />}
        />
        <Route
          path="/products/add"
          element={loggedIn ? <ProductAdd onLogout={handleLogout} /> : <Navigate to="/login" />}
        />
        <Route
          path="/invoices"
          element={loggedIn ? <Invoice onLogout={handleLogout} /> : <Navigate to="/login" />}
        />
      </Routes>
    </Router>
  );
};

export default App;