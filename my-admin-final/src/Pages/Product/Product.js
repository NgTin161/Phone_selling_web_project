import React, { useEffect, useState } from 'react'
import Layout from '../../Component/Layouts/Layout'
import axiosClient from '../../Component/axiosClient';
import { Button, Modal, Pagination, Table } from 'react-bootstrap';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faBox, faCheck, faCirclePlus, faEdit, faTimes, faTrash } from '@fortawesome/free-solid-svg-icons';
import { Link } from 'react-router-dom';

const itemsPerPage = 10;

const Product = () => {
    const [products, setProducts] = useState([]);
    const [productModel, setProductModel] = useState([]);
    const [currentPage, setCurrentPage] = useState(1);
    const [selectedProduct, setSelectedProduct] = useState([]);
    const [selectedProductModel, setSelectedProductModel] = useState([]);
    const [show, setShow] = useState(false);
    const [showDelete, setShowDelete] = useState(false);
    const maxLength = 50;
    
    useEffect(() => {
        axiosClient.get('/phones')
            .then((res) => {
                setProducts(res.data);
            })
            .catch((error) => {
                console.error("Error fetching data:", error);
            });
    }, []);

    useEffect(() => { 
        axiosClient.get('/phonemodels')
            .then(res => {
                setProductModel(res.data);
            })
            .catch((error) => {
                console.error("Error fetching data:", error)
            });
    }, []);

    const truncateDescription = (desc) => {
        return desc.length > maxLength ? `${desc.substring(0, maxLength)}...` : desc;
    }

    const handleShow = (id) => {
        const selected = products.find(p => p.id === id);
        const selectedModel = productModel.filter(p => p.phoneId === id)[0];
        setSelectedProduct(selected);
        setSelectedProductModel(selectedModel);
        setShow(true);
    };
    const handleClose = () => setShow(false);

    const handleShowDelete = (id) => {
        setShowDelete(products.find(p => p.id === id));
        setShowDelete(true);
    };
    const handleCloseDelete = () => setShowDelete(false);

    const handleDelete = (id) => {
        axiosClient.delete(`/phones/${selectedProduct.id}`);
        setShowDelete(false);
    };

    const totalPages = Math.ceil(products.length / itemsPerPage);

    const startIndex = (currentPage - 1) * itemsPerPage;
    const endIndex = startIndex + itemsPerPage;
    const currentData = products.slice(startIndex, endIndex);

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
    <Layout>
        <div className="product-container">
            <h2>Sản phẩm</h2>
            <Link to={`/products/add`} className="btn btn-success" style={{ textDecoration: 'none' }}>
                <FontAwesomeIcon icon={faCirclePlus} flip size="2xl" style={{ marginRight: '10px' }} />
                <span style={{ marginRight: '10px' }}>Thêm sản phẩm</span>
            </Link>
            <table>
                <thead>
                    <tr>
                        <th>STT</th>  
                        <th>Ảnh</th>  
                        <th>Tên</th>
                        <th>Mô tả</th>
                        <th>Tồn kho</th>
                        <th>Chức năng</th>
                    </tr>
                </thead>
                <tbody>
                      {currentData.map(item => {
                          return (
                              <tr key={item.id}>
                                  <td>{item.id}</td>
                                  <td>{item.thumbnail && (
                                    <img src={`https://localhost:7186/Thumbnail/${item.thumbnail}`} style={{width: "100px"}}/>
                                  )}
                                  </td>
                                  <td>{item.name}</td>
                                  <td>{truncateDescription(item.description)}</td>
                                  <td>{item.stock}</td>
                                  <td>
                                      <div style={{ display: 'flex',justifyContent:'center', gap: '10px' }}>    
                                        <Button variant="info" onClick={() => handleShow(item.id)}>
                                            <FontAwesomeIcon icon={faBox} />
                                        </Button>
                                        <Link to={`/products/edit/${item.id}`} style={{ textDecoration: 'none' }}>
                                            <Button variant="warning">
                                                <FontAwesomeIcon icon={faEdit} />
                                            </Button>
                                        </Link>
                                        <Button variant="danger" onClick={() => handleShowDelete(item.id)}>
                                            <FontAwesomeIcon icon={faTrash} />
                                        </Button>
                                      </div>
                                  </td>               
                              </tr>
                          )
                      })}
                </tbody>
            </table>
            <div>
                <Modal show={show} size="lg" onHide={handleClose} centered>
                    <Modal.Header closeButton>
                        <Modal.Title>Sản phẩm</Modal.Title>
                    </Modal.Header>
                    <Modal.Body>
                        <h2>{selectedProductModel && selectedProductModel.phone && selectedProductModel.phone.name}</h2>
                        <Table bordered striped>
                            <tbody>
                                <tr>
                                    <td>
                                        <span style={{ marginLeft: "10px" }}>Kích thước màn hình</span>
                                        <span style={{ marginLeft: "520px" }}>{selectedProductModel && selectedProductModel.screenSize}" </span>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <span style={{ marginLeft: "10px" }}>Hệ điều hành</span>
                                        <span style={{ marginLeft: "550px" }}>{selectedProductModel && selectedProductModel.os}</span>
                                    </td>      
                                </tr>
                                <tr>
                                    <td>      
                                          <span style={{ marginLeft: "10px"}}>Camera</span>
                                        <span style={{ marginLeft: "590px" }}>{selectedProductModel && selectedProductModel.cameraResolution} MP</span>
                                    </td>
                                </tr>
                                <tr>
                                    <td>      
                                        <span style={{ marginLeft: "5px" }}>Chip</span>
                                        <span style={{ marginLeft: "620px" }}>{selectedProductModel && selectedProductModel.cpu}</span>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <span style={{ marginLeft: "10px" }}>Ram</span>
                                        <span style={{ marginLeft: "620px" }}>{selectedProductModel && selectedProductModel.ram && selectedProductModel.ram.name} GB</span>
                                    </td>      
                                </tr>
                                <tr>
                                    <td>  
                                        <span style={{ marginLeft: "15px" }}>Dung lượng lưu trữ</span>
                                        <span style={{ marginLeft: "520px" }}>{selectedProductModel && selectedProductModel.storage && selectedProductModel.storage.name}</span>
                                    </td>      
                                </tr>
                                <tr>
                                    <td>
                                        <span style={{ marginLeft: "16px" }}>Dung lượng pin</span>
                                        <span style={{ marginLeft: "530px" }}>{selectedProductModel && selectedProductModel.batteryCapacity} mAh</span>
                                    </td>      
                                </tr>
                                <tr>
                                    <td>
                                        <span style={{ marginLeft: "28px" }}>Màu</span>
                                        <span style={{ marginLeft: "590px" }}>{selectedProductModel && selectedProductModel.color && selectedProductModel.color.name}</span>
                                    </td>      
                                </tr>
                                <tr>
                                    <td>      
                                        <span>Cổng sạc</span>
                                        <span style={{ marginLeft: "570px" }}>{selectedProductModel && selectedProductModel.chargingPort}</span>
                                    </td>      
                                </tr>
                                <tr>
                                    <td>
                                        <span style={{ marginLeft: "25px" }}>Giá</span>
                                        <span style={{ marginLeft: "580px" }}>{selectedProductModel && selectedProductModel.price} VND</span>
                                    </td>
                                </tr>
                            </tbody>      
                        </Table>
                    </Modal.Body>
                    <Modal.Footer>
                        <Button variant="secondary" onClick={handleClose}>
                            Close
                        </Button>
                    </Modal.Footer>
                </Modal>
            </div>
            <div>
                <Modal show={showDelete} onHide={handleCloseDelete} centered>
                    <Modal.Header closeButton>
                    <Modal.Title>Xác nhận xoá</Modal.Title>
                    </Modal.Header>
                            <Modal.Body>Bạn có chắc muốn xoá sản phẩm <span style={{fontWeight:"bold"}}>{selectedProduct.name}</span> này?</Modal.Body>
                    <Modal.Footer>
                    <Button variant="danger" onClick={handleDelete}>
                        <FontAwesomeIcon icon={faCheck}/> Yes
                    </Button>
                    <Button variant="secondary" onClick={handleCloseDelete}>
                        <FontAwesomeIcon icon={faTimes}/> No
                    </Button>
                    </Modal.Footer>
                </Modal>
            </div>
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
    </Layout>
  )
}

export default Product;