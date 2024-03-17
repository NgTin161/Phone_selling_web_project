import React, { useState, useEffect } from 'react';
import './customerstate.css';

const CustomerStats = () => {
  const [newCustomers, setNewCustomers] = useState(0);
  const [loyalCustomers, setLoyalCustomers] = useState(0);
  const [customerData, setCustomerData] = useState([]);

  const mockCustomerData = [
    { id: 1, name: 'John Doe', age: 30, location: 'New York', gender: 'Male' },
    { id: 2, name: 'Jane Smith', age: 25, location: 'Los Angeles', gender: 'Female' },
    { id: 3, name: 'Bob Johnson', age: 35, location: 'Chicago', gender: 'Male' },
  ];

  useEffect(() => {
    const newCustomersCount = mockCustomerData.filter((customer) => customer.id % 2 === 0).length;
    const loyalCustomersCount = mockCustomerData.filter((customer) => customer.id % 2 !== 0).length;

    setNewCustomers(newCustomersCount);
    setLoyalCustomers(loyalCustomersCount);
    setCustomerData(mockCustomerData);
  }, []);

    return (  
        <div className="customer-stats">
            <h2>Customer Statistics</h2>
            <div className="stats-item">
                <p>New Customers: {newCustomers}</p>
            </div>
            <div className="stats-item">
                <p>Loyal Customers: {loyalCustomers}</p>
            </div>
            <div className="customer-list">
                <h3>Customer List</h3>
                <ul>
                {customerData.map((customer) => (
                    <li key={customer.id}>
                    {customer.name} - {customer.age} years old - {customer.location} - {customer.gender}
                    </li>
                ))}
                </ul>
            </div>
        </div>
    );
};

export default CustomerStats;
