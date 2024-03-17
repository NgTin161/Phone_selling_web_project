import React, { useState } from 'react'
import Layout from '../../Component/Layouts/Layout'
import { Link, useNavigate } from 'react-router-dom';
import { Button } from 'react-bootstrap';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faArrowCircleLeft, faCheck } from '@fortawesome/free-solid-svg-icons';
import axiosClient from '../../Component/axiosClient';

const UserAdd = () => {
  const navigate = useNavigate();
  const [account, setAccount] = useState({
    username: '',
    password: '',
    email: '',
    fullname: ''
  });
  const [error, setError] = useState('');
  
  const handleChange = (e) => {
    let name = e.target.name;
    let value = e.target.value;

    setAccount(prev => ({ ...prev, [name]: value }));
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    
    axiosClient.post('/users/register-admin', account)
      .then(() => {
        console.log("Create complete");
        navigate('/users');
      })
      .catch((error) => {
        console.error("Error adding user:", error);
        if (error.response && error.response.status === 500) {
          setError("Có lỗi xảy ra trong quá trình tạo tài khoản.");
        }
      });
  };

  const isDataValid = () => { 
    return (
      account.username && account.password && account.email && account.fullname !== null
    );
  }

  return (
    <Layout>
      <h2>Thêm tài khoản quản lý</h2>
      {error && <div className="alert alert-danger">{error}</div>}
      <form className="col-md-3" onSubmit={handleSubmit}>
        <div className="mb-3">
          <label>Username:</label>
          <input type="text" name="username" className="form-control" value={account.username || ''} onChange={handleChange} />
        </div>
        <div className="mb-3">
          <label>Password:</label>
          <input type="password" name="password" className="form-control" value={account.password || ''} onChange={handleChange} />
        </div>
        <div className="mb-3">
          <label>Email:</label>
          <input type="email" name="email" className="form-control" value={account.email || ''} onChange={handleChange} />
        </div>
        <div className="mb-3">
          <label>Fullname:</label>
          <input type="text" name="fullname" className="form-control" value={account.fullname || ''} onChange={handleChange} />
        </div>
        <Button type="submit" variant="success" disabled={!isDataValid()}>
          <FontAwesomeIcon icon={faCheck} style={{ marginRight: '10px' }} />
          <span style={{ marginRight: '10px' }}>Thêm</span>
        </Button>
        <Link to="/users" className="btn btn-secondary" style={{marginLeft: "10px", marginTop: "13px", width: "130px", height: "38px"}}>
          <FontAwesomeIcon icon={faArrowCircleLeft} style={{ marginRight: '10px', textDecoration: 'none' }} />
          <span style={{ marginRight: '10px' }}>Quay lại</span>
        </Link>
      </form>
    </Layout>    
  )
}

export default UserAdd;
