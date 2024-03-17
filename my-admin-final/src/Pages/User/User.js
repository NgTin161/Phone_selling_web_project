import React, { useEffect, useState } from 'react'
import Layout from '../../Component/Layouts/Layout'
import axiosClient from '../../Component/axiosClient';
import { Link, useNavigate } from 'react-router-dom';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faCheck, faCirclePlus, faTimes, faTrash } from '@fortawesome/free-solid-svg-icons';
import { Button, Modal } from 'react-bootstrap';

const User = () => {
    const [users, setUsers] = useState([]);
    const navigate = useNavigate();
    const [showDelete, setShowDelete] = useState(false);
    const [selectedUser, setSelectedUser] = useState([]);
    
    useEffect(() => {
        axiosClient.get('/users')
            .then(res => {
            
                setUsers(res.data);
            })
            .catch(error => {

                console.error("Fetching error data:", error);

            });
    }, []);

    const handleShowDelete = (username) => {
        setSelectedUser(users.find(u => u.userName === username));
        setShowDelete(true);
    }
    const handleCloseDelete = () => setShowDelete(false);

    const handleDelete = (username) => {
        axiosClient.delete(`/users/${selectedUser.userName}`);
        navigate('/users');
        console.log("Delete complete");
        setShowDelete(false);
    }
    
    return (
        <Layout>
            <h2>Tài khoản</h2>
            <Link to={`/users/register-admin`} className="btn btn-success" style={{ textDecoration: 'none' }}>
                <FontAwesomeIcon icon={faCirclePlus} flip size="2xl" style={{ marginRight: '10px' }} />
                <span style={{ marginRight: '10px' }}>Thêm tài khoản</span>
            </Link>
            <table>
                <thead>
                    <tr>
                        <th>STT</th>
                        <th>Tài khoản</th>
                        <th>Họ và tên</th>
                        <th>Email</th>
                        <th>Vai trò</th>
                        <th>Hoạt động</th>
                        <th>Chức năng</th>
                    </tr>
                </thead>
                <tbody>
                    {users.map((item, index) => {
                        return (
                            <tr key={item.id || index}>
                                <td>{index + 1}</td>
                                <td>{item.userName}</td>
                                <td>{item.fullName}</td>
                                <td>{item.email}</td>
                                <td>{item.roles ? item.roles.join(', ') : " "}</td>
                                <td>{item.isDelete ? "Đã xoá" : "Còn hoạt động"}</td>
                                <td>
                                    <Button variant="danger" onClick={() => handleShowDelete(item.userName)}>
                                        <FontAwesomeIcon icon={faTrash}></FontAwesomeIcon>
                                    </Button>
                                </td>
                            </tr>
                        )
                    })
                    }
                </tbody>
            </table>
            <Modal show={showDelete} onHide={handleCloseDelete} centered>
                <Modal.Header closeButton>
                    <Modal.Title>Xác nhận xoá</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    Bạn có chắc muốn xoá tài khoản <span style={{ fontWeight: "bold" }}>{selectedUser.userName}</span> này?
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="danger" onClick={handleDelete}>
                        <FontAwesomeIcon icon={faCheck}/> Yes
                    </Button>
                    <Button variant="secondary" onClick={handleCloseDelete}>
                        <FontAwesomeIcon icon={faTimes}/> No
                    </Button>
                </Modal.Footer>
            </Modal>
        </Layout>
    )
};

export default User;
