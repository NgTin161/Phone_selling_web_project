import React, { useEffect, useState } from 'react'
import { Link, useNavigate } from 'react-router-dom';
import axiosClient from '../../Component/axiosClient';
import { Button } from 'react-bootstrap';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import Layout from '../../Component/Layouts/Layout';
import { faArrowCircleLeft, faCheck } from '@fortawesome/free-solid-svg-icons';

const ProductAdd = () => {
    const navigate = useNavigate();
    const [product, setProduct] = useState({
        SKU: '',
        name: '',
        description: '',
        stock: '',
        productTypeId: '',
        status: false,
        thumbnail: null,
    });
    const [productType, setProductType] = useState([]);
    const [image, setImage] = useState(null);
    const [error, setError] = useState('');

    useEffect(() => {
        axiosClient.get('/productTypes')
            .then(res => setProductType(res.data))
            .catch(error => console.error("Error fetching product types:", error));
    }, []);

    const handleChange = (e) => {
        let name = e.target.name;
        let value = e.target.value;
        setProduct(prev => ({ ...prev, [name]: value }));
    };

    const handleCheck = (e) => {
        let name = e.target.name;
        let value = e.target.checked;
        setProduct(prev => ({ ...prev, [name]: value }));
    };

    const handleImageChange = (e) => {
        const file = e.target.files[0];
        setImage(file);

        if (file) {
            const reader = new FileReader();
            reader.onloadend = () => {
                setImage(reader.result);
            };
            reader.readAsDataURL(file);
        } else {
            setImage(null);
        }
    }

    const handleSubmit = async (e) => {
        e.preventDefault();

        // if (!product.SKU || !product.productName || !product.productTypeId || !image) {
        //     setError('Vui lòng điền đầy đủ thông tin.');
        //     return;
        // }

        const formData = new FormData();
        formData.append('thumbnail', image);

        Object.keys(product).forEach(key => {
            formData.append(key, product[key]);
        });
        await axiosClient.post("/phones", product)
            .then(res => {
                console.log("Create complete");
                navigate("/products");
            })
            .catch(error => {
                console.error("Error adding product: ", error);
                if (error.response && error.response.status === 500) {
                    setError('Đã xảy ra lỗi khi thêm sản phẩm.');
                }
            });
    }

    const isDataValid = () => {
        return (
            product.name && product.description && product.stock && product.SKU !== null && product.productTypeId !== '' && image !== null
        );
    }

    return (
        <Layout>
            <h2>Thêm sản phẩm mới</h2>
            <form className="col-md-3" encType="multipart/form-data">
                {error && <div className="alert alert-danger">{error}</div>}
                <div className="mb-3">
                    <label>SKU</label>
                    <input type="text" className="form-control" name="SKU" value={product.SKU || ''} onChange={handleChange} />
                </div>
                <div className="mb-3">
                    <label>Tên sản phẩm</label>
                    <input type="text" className="form-control" name="name" value={product.name || ''} onChange={handleChange}/>
                </div>
                <div className="mb-3">
                    <label>Mô tả</label>
                    <textarea
                        className="form-control"
                        name="description"
                        value={product.description || ''}
                        onChange={handleChange}
                    />
                </div>
                <div className="mb-3">
                    <label>Tồn kho</label>
                    <input type="number" className="form-control" name="stock" value={product.stock || ''} onChange={handleChange}/>
                </div>
                <div className="mb-3">
                    <label>Loại sản phẩm</label>
                    <select name="productTypeId" className="form-control" value={product.productTypeId} onChange={handleChange}>
                        <option value="">Chọn loại sản phẩm</option>
                        {Array.isArray(productType) && productType.map(type => (
                            <option key={type.id} value={type.id}>
                                {type.name}
                            </option>
                        ))}
                    </select>
                </div>
                <div className="mb-3">
                    <label>Hình ảnh</label>
                    {image && <img src={image} alt="Product Image"/>}
                    <input type="file" name="image" onChange={handleImageChange} />
                </div>
                <div className="mb-3">
                    <label style={{ marginRight: '10px' }}>
                        <span style={{ marginRight: '10px' }}>Còn hoạt động</span>
                        <input
                            type="checkbox"
                            name="status"
                            onChange={handleCheck}
                            checked={product.status || false}
                        />
                    </label>
                </div>
                <Button type="submit" variant="success" onClick={handleSubmit} disabled={!isDataValid()}><FontAwesomeIcon icon={faCheck} style={{ marginRight: '10px' }} /><span style={{ marginRight: '10px' }}>Thêm</span></Button>
                <Link to="/products" className="btn btn-secondary" style={{marginLeft: "10px", marginTop: "13px", width: "130px", height: "38px"}}>
                    <FontAwesomeIcon icon={faArrowCircleLeft} style={{ marginRight: '10px', textDecoration: 'none' }} />
                        <span style={{ marginRight: '10px' }}>Quay lại</span>
                </Link>
            </form>
        </Layout>
    );
};

export default ProductAdd;
