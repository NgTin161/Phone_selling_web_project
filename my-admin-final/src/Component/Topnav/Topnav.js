import React, { useEffect, useRef, useState } from 'react';
import './topnav.css'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faPowerOff, faSearch, faSignOutAlt } from '@fortawesome/free-solid-svg-icons';
import { jwtDecode } from 'jwt-decode';
import { useNavigate } from 'react-router-dom';


const Topnav = ({ onLogout }) => {
   const [userName, setUserName] = useState(null);
  const [searchFocused, setSearchFocused] = useState(false);
  const [logoutVisible, setLogoutVisible] = useState(false);
  const searchInputRef = useRef(null);
  const navigate = useNavigate();

  const handleLogout = () => {
    localStorage.removeItem('jwt');
    if (typeof onLogout === 'function') {
      onLogout();
    }
    navigate('/login');
  };

  useEffect(() => {
  const token = localStorage.getItem('jwt');

  if (token) {
    try {
      const decodedUserInfo = jwtDecode(token);
      const userName = decodedUserInfo['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'];

      if (userName) {
        setUserName(userName);
      } else {
        console.error('User Name not found in decoded user info.');
      }
    } catch (error) {
      console.error('Error decoding token:', error);
    }
  }
}, []);

  useEffect(() => {
    const handleKeyPress = (e) => {
      if (e.key === 'Enter') {
        console.log('Searching for:', searchInputRef.current.value);
      }
    };

    document.addEventListener('keypress', handleKeyPress);

    return () => {
      document.removeEventListener('keypress', handleKeyPress);
    };
  }, []);

  useEffect(() => {
      const handleClickOutside = (event) => {
        if (logoutVisible && !event.target.closest('.user-container')) {
          setLogoutVisible(false);
        }
      };

      document.addEventListener('click', handleClickOutside);

      return () => {
        document.removeEventListener('click', handleClickOutside);
      };
    }, [logoutVisible]);

  const handleMouseEnter = () => {
    setSearchFocused(true);
  };

  const handleMouseLeave = () => {
    setSearchFocused(false);
  };

  const handleUserNameClick = () => {
    setLogoutVisible(!logoutVisible);
  };

  return (
    <nav id="topnav">
      <div id="search-bar" className={searchFocused ? 'focused' : ''} onMouseEnter={handleMouseEnter} onMouseLeave={handleMouseLeave}>
        <input type="text" ref={searchInputRef} placeholder="Tìm kiếm..." onKeyDown={(e) => e.key === 'Enter' && e.preventDefault()} />
        <button id="search-btn">
          <FontAwesomeIcon icon={faSearch} />
        </button>
      </div>
      {userName ? (
  <div className={`user-container ${logoutVisible ? 'logout-visible' : ''}`} onClick={handleUserNameClick}>   
    <div className="avatar"></div>  
    <h2 className="user-name">{userName}</h2>
    {logoutVisible && (
      <div id="logout-sidebar">
        <a className="dropdown-item" onClick={handleLogout}>
          <i className="dropdown-item-icon mdi mdi-power text-black me-2">
            <FontAwesomeIcon icon={faPowerOff} />
          </i> <span style={{color:"black"}}>Đăng xuất</span>
        </a>
      </div>
    )}
  </div>
) : (
  <Link to="/login">Đăng nhập</Link>
)}

    </nav>
  );
};

export default Topnav;
