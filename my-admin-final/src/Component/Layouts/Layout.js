import React from 'react'
import Topnav from '../Topnav/Topnav'
import Sidebar from '../Sidebar/Sidebar'
import './layout.css'

const Layout = ({ children, userInfo, onLogout }) => {
  return (
    <div id="layout">
      <Sidebar />
      <div id="content">
        <Topnav userInfo={userInfo} onLogout={onLogout}/>
        <div id = "main">
            {children}       
        </div>
      </div>
    </div>
  )
}

export default Layout
