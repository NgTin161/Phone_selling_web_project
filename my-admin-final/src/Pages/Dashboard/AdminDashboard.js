import React from 'react';
import Layout from '../../Component/Layouts/Layout';
import ProductChart from '../../Component/Management/ProductChart/ProductChart';
import CustomerStats from '../../Component/Management/CustomerState/CustomerStates';
import './admindashboard.css'
import OrderStatus from '../../Component/Management/OrderStatus/OrderStatus';

const AdminDashboard = () => {
  return (
      <Layout>
        <h2>Dashboard</h2>
        <div className='dashboard-container'>
          <div className="left-panel">
            <ProductChart />
          </div>
          <div className="right-panel">
            <CustomerStats />
          </div>
          <div className="left-panel">
            <OrderStatus />
          </div>
        </div>
      </Layout>
  );
};

export default AdminDashboard;