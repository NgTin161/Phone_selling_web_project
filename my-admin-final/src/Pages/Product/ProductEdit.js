import React, { useEffect, useState } from 'react'
import axiosClient from '../../Component/axiosClient';
import Layout from '../../Component/Layouts/Layout';
import { Link, useNavigate, useParams } from 'react-router-dom';
import { Button } from 'react-bootstrap';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faArrowCircleLeft, faCheck } from '@fortawesome/free-solid-svg-icons';
import './producteditadd.css';

const ProductEdit = () => {
    const navigate = useNavigate();
    const { id } = useParams();
    const [product, setProduct] = useState({});
    const [productType, setProductType] = useState({});
    const [image, setImage] = useState(null);

    useEffect(() => { 
        axiosClient.get('/productTypes')
            .then(res => setProductType(res.data))
            .catch(error => console.error("Error fetching data:",error));
    }, []);

    useEffect(() => { 
        axiosClient.get(`/phones/${id}`)
            .then(res => {
                setProduct(res.data);
                if (res.data.thumbnail) {
                    setImage(`https://localhost:7186/Thumbnail/${res.data.thumbnail}`);
                }
            })
            .catch((error) => console.error("Error fetching data:", error));
    }, [id]);

    const handleChange = (e) => {
        let name = e.target.name;
        let value = e.target.value;
        setProduct(prev => ({ ...prev, [name]: value }));
    }

    const handleCheck = (e) => {
        let name = e.target.name;
        let value = e.target.checked;
        setProduct(prev => ({ ...prev, [name]: value }));
    }

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

        const formData = new FormData();
        formData.append('thumbnail',image);

        Object.keys(product).forEach(key => { 
            formData.append(key, product[key]);
        });

        try {
            await axiosClient.put(`/phones/${id}`, product);
            navigate("/products");
        }
        catch (error){
            console.error("Error fetching data:", error);
        }
        console.log("Updating complete");
    }

    return (
        <Layout>
            <h2>Sửa sản phẩm</h2>
            <form className="col-md-3" encType="multipart/form-data">
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
                    <select name="productType" className="form-control" value={product.productTypeId} onChange={handleChange}>
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
                <Button type="submit" variant="success" onClick={handleSubmit} ><FontAwesomeIcon icon={faCheck} style={{ marginRight: '10px' }} /><span style={{ marginRight: '10px' }}>Cập nhật</span></Button>
                <Link to="/products" className="btn btn-secondary" style={{marginLeft: "10px", marginTop: "13px", width: "130px", height: "38px"}}>
                    <FontAwesomeIcon icon={faArrowCircleLeft} style={{ marginRight: '10px', textDecoration: 'none' }} />
                        <span style={{ marginRight: '10px' }}>Quay lại</span>
                </Link>
            </form>
        </Layout>
    );
};

export default ProductEdit;
