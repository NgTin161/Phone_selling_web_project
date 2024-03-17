import React, { useEffect, useState } from 'react'
import './invoice.css'
import axiosClient from '../../Component/axiosClient';
import Layout from '../../Component/Layouts/Layout';
import { Button, Modal } from 'react-bootstrap';

const Invoice = () => {
    const [invoices, setInvoices] = useState([]);
    const [invoiceDetails, setInvoiceDetails] = useState([]);
    const [phoneModels, setPhoneModels] = useState([]);
    const [show, setShow] = useState(false);
    const [selectedInvoices, setSelectedInvoices] = useState([]);
    const [selectedInvoiceDetails, setSelectedInvoiceDetails] = useState([]);
    const [selectedOption, setSelectedOption] = useState("");
    const [selectedModels, setSelectedModels] = useState([]);

    useEffect(() => { 
        axiosClient.get('/invoices')
            .then((res) => {
                const invoicesWithStatus = res.data.map(item => ({ ...item, statusInvoice: "approved" }));
                setInvoices(invoicesWithStatus);
            })
            .catch((error) => {
                console.error("Error fetching data:", error)
            });
    }, []);

    useEffect(() => { 
        axiosClient.get('/invoiceDetails')
            .then((res) => {
                setInvoiceDetails(res.data);
            })
            .catch((error) => {
                console.error("Error fetching data:", error)
            });
    }, []);

    useEffect(() => { 
        axiosClient.get('/phonemodels')
            .then((res) => {
                setPhoneModels(res.data);
            })
            .catch((error) => {
                console.error("Error fetching data:", error)
            });
    }, []);

    const handleStatusChange = async (invoiceId, newStatus) => {
        try {
            await axiosClient.patch(`/invoices/statusInvoice/${invoiceId}`, { statusInvoice: newStatus }, {
                headers: {
                    'Content-Type': 'application/json',
                },
            });
            const updatedInvoiceList = invoices.map(invoiceItem => {
                if (invoiceItem.id === invoiceId) {
                    return { ...invoiceItem, StatusInvoice: newStatus };
                }
                return invoiceItem;
            });
            
            setInvoices(updatedInvoiceList);
            setSelectedOption(newStatus);
        } catch (error) {
            console.error('Error updating status:', error);
        }
    };

    const handleShow = (id) => {
        const selected = invoices.find(i => i.id === id);
        const selectedModels = phoneModels.find(i => i.id === id);
        const selectedDetails = invoiceDetails.filter(i => i.invoiceId === id && i.phoneModelId === id)[0];
        setSelectedInvoices(selected);
        setSelectedModels(selectedModels);
        setSelectedInvoiceDetails(selectedDetails);
        setShow(true);
    };
    const handleClose = () => setShow(false);
        
    return (
        <Layout>
            <h2>Hoá đơn</h2>
                {
                    invoices.map(item => {
                        return (
                            <ul className="row invoice-container"  key={item.id}>
                                <li className="col-md-3">
                                    <p>Mã đơn hàng: {item.code}</p>
                                    <p>Tên khách hàng: {item.user.fullName}</p>
                                    <p>Ngày đặt hàng: {item.issuedDate}</p>
                                    <p>Địa chỉ giao hàng: {item.shippingAddress}</p>
                                    <div>
                                        <label>
                                            <span style={{marginRight:"5px", color:"orange"}}> Đã duyệt </span>
                                            <input
                                                type="radio"
                                                checked={selectedOption === "approved"}
                                                style={{marginRight:"10px"}}
                                                onChange={() => handleStatusChange(item.id, "approved")}                                                
                                            />
                                        </label>
                                        <label>
                                            <span style={{marginRight:"5px",color:"green"}}> Đã thanh toán </span>
                                            <input
                                                type="radio"
                                                checked={selectedOption === "paid"}
                                                style={{ marginRight: "10px" }}
                                                onChange={() => handleStatusChange(item.id, "paid")}
                                            />
                                        </label>
                                        <label>
                                        <span style={{marginRight:"5px", color:"red"}}> Đã huỷ </span>
                                            <input
                                                type="radio"
                                                checked={selectedOption === "cancelled"}
                                                onChange={() => handleStatusChange(item.id, "cancelled")}
                                            />
                                        </label>       
                                    </div>
                                    <Button variant="info" onClick={() => handleShow(item.id)}>
                                        Xem chi tiết   
                                    </Button>
                                </li>
                            </ul>
                        )
                    })
                }
            <Modal show={show} size="lg" onHide={handleClose} centered>
                <Modal.Body>
                    <h2>Chi tiết hoá đơn [{selectedInvoices && selectedInvoices.code}]</h2>
                    <p>{selectedInvoices && selectedInvoices.shippingAddress }</p>
                    <p>Tên khách hàng: {selectedInvoices && selectedInvoices.user && selectedInvoices.user.fullName}</p>
                    <p>SĐT: {selectedInvoices && selectedInvoices.shippingPhone}</p>
                    <span style={{ marginRight: "450px" }}>Tên sản phẩm</span>
                    <span style={{ marginRight: "80px" }}>SL</span>
                    <span style={{ marginRight: "10px" }}>Giá tiền</span>
                    <hr />
                    <span style={{ marginRight: "420px" }}>{selectedModels && selectedModels.phone && selectedModels.phone.name}</span>
                    <span style={{ marginRight: "90px" }}>{selectedInvoiceDetails && selectedInvoiceDetails.quantity}</span>
                    <span style={{ marginRight: "10px" }}>{selectedInvoiceDetails && selectedInvoiceDetails.unitPrice}</span>
                    <hr />
                    <span style={{marginRight:"500px"}}>Tổng tiền</span>
                    <span style={{ marginLeft: "70px", marginRight: "30px" }}>{selectedInvoices && selectedInvoices.total} VND</span>
                </Modal.Body>
                <Modal.Footer>
                    
                    <Button variant="secondary" onClick={handleClose}>
                            Close
                    </Button>
                </Modal.Footer>
            </Modal>
        </Layout>
    );
};

export default Invoice;
