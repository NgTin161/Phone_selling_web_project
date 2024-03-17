import React, { useState } from 'react'
import './orderstatus.css'
import { Pagination } from 'react-bootstrap';

const itemsPerPage = 5;

const OrderStatus = () => {
    const [currentPage, setCurrentPage] = useState(1);

    const orderData = [
        { id: 1, status: 'Đã thanh toán', count: 30 },
        { id: 2, status: 'Chưa thanh toán', count: 20 },
        { id: 3, status: 'Chưa thanh toán', count: 20 },
        { id: 4, status: 'Chưa thanh toán', count: 20 },
        { id: 5, status: 'Chưa thanh toán', count: 20 },
        { id: 6, status: 'Đã giao hàng', count: 50 },
        { id: 7, status: 'Đã huỷ', count: 10 },
        { id: 8, status: 'Đã huỷ', count: 10 },
        { id: 9, status: 'Đã huỷ', count: 10 },
        { id: 10, status: 'Đã thanh toán', count: 30 },
        { id: 11, status: 'Đã thanh toán', count: 30 },
        { id: 12, status: 'Đã thanh toán', count: 30 },
        { id: 13, status: 'Đã thanh toán', count: 30 },
    ];

    const totalPages = Math.ceil(orderData.length / itemsPerPage);

    const startIndex = (currentPage - 1) * itemsPerPage;
    const endIndex = startIndex + itemsPerPage;
    const currentData = orderData.slice(startIndex, endIndex);

    const handlePageChange = (page) => { 
        setCurrentPage(page);
    };

    const handlePreviousPage = () => {
        if (currentPage > 1) {
        setCurrentPage(currentPage - 1);
        }
    };

    const handleNextPage = () => {
        if (currentPage < totalPages) {
        setCurrentPage(currentPage + 1);
        }
    };

    return (
        <div className="order-status-table">
            <h2>Tình trạng đơn hàng</h2>
            <table>
                <thead>
                    <tr>
                        <th>STT</th>
                        <th>Số lượng</th>
                        <th>Tình trạng</th>
                    </tr>
                </thead>
                <tbody>
                    {currentData.map((order) => (
                        <tr key={order.id}>
                            <td>{order.id}</td>
                            <td>{order.count}</td>
                            <td>{order.status}</td>
                        </tr>
                    ))}
                </tbody>
            </table>
            <div className="pagination-container">
                <Pagination>
                    <Pagination.Prev onClick={handlePreviousPage} disabled={currentPage === 1} />
                    {Array.from({ length: totalPages }, (_, index) => index + 1).map((page) => (
                        <Pagination.Item
                        key={page}
                        active={page === currentPage}
                        onClick={() => handlePageChange(page)}
                        >
                        {page}
                        </Pagination.Item>
                    ))}
                    <Pagination.Next onClick={handleNextPage} disabled={currentPage === totalPages} />
                </Pagination>
            </div>
        </div>
    );
};

export default OrderStatus;
