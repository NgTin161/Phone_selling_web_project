    import { faCheck } from "@fortawesome/free-solid-svg-icons";
    import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
    import { useEffect, useState } from "react";
    import { Alert, Button, Form, FormGroup } from "react-bootstrap";
    import axiosClient from '../../Component/axiosClient'
    import { useNavigate } from "react-router-dom";
    import './login.css';

    const Login = ({ setLoggedIn }) => {
        const [account, setAccount] = useState({});
        const [error, setError] = useState(null);
        const [userStartedTyping, setUserStartedTyping] = useState(false);
        const navigate = useNavigate();

        useEffect(() => {
            if (userStartedTyping) {
                setError(null);
            }
        }, [account, userStartedTyping]);
        
        useEffect(() => {
            const token = localStorage.getItem('jwt');
            if (token) {
                setLoggedIn(true);
                navigate('/');
            }
        }, [setLoggedIn, navigate]);

        const handleChange = (e) => {
            setAccount(prev => ({...prev, [e.target.name]: e.target.value }));
        }

        const handleSubmit = (e) => {
            e.preventDefault();

            if (!account.username || !account.password) {
                setError("Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu.");
                return;
            }
            else {
                axiosClient.post(`/Users/login`, account)
                    .then(res => {
                        localStorage.setItem("jwt", res.data.token);
                        navigate('/');
                    })
                    .catch(error => {
                        console.error(error);
                        if (error.response && error.response.status === 401) {
                            setError("Tên đăng nhập hoặc mật khẩu không chính xác.");
                        }
                    });                           
            }
        }

        return (
            <div className="container">
                <h2>
                    ADMIN
                </h2>
                {error && <Alert variant="danger">{error}</Alert>}
                <Form onSubmit={handleSubmit}>
                    <FormGroup>
                        <Form.Label>Username</Form.Label>
                        <Form.Control type="text" name="username" onChange={handleChange}/>
                    </FormGroup>
                    <FormGroup>
                        <Form.Label>Password</Form.Label>
                        <Form.Control type="password" name="password" onChange={handleChange}/>
                    </FormGroup>
                    <Button type="submit" variant="success" className="submit">
                        <FontAwesomeIcon icon={faCheck} /> Submit
                    </Button>
                </Form>
            </div>
        )
    }

    export default Login;