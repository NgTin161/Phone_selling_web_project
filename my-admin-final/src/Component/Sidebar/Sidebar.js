import React from 'react';
import { Link } from 'react-router-dom';
import './sidebar.css'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faBasketShopping, faMobileAlt, faTachometerAlt, faUser } from '@fortawesome/free-solid-svg-icons';

const Sidebar = () => {
  return (
    <nav id="sidebar" className="active">
      <div className="sidebar-header">
        <h3>ADMIN</h3>
      </div>
      <ul className="list-unstyled components">
        <li>
          <Link to="/">
            <span className="icon"><FontAwesomeIcon icon={faTachometerAlt} /></span> Dashboard
          </Link>
          <Link to="/users">
            <span className="icon"><FontAwesomeIcon icon={faUser} /></span> Users
          </Link>
          <Link to="/products">
            <span className="icon"><FontAwesomeIcon icon={faMobileAlt} /></span> Products
          </Link>
          <Link to="/invoices">
            <span className="icon"><FontAwesomeIcon icon={faBasketShopping} /></span> Invoices
          </Link>
        </li>
        
      </ul>
    </nav>
  );
};

export default Sidebar;