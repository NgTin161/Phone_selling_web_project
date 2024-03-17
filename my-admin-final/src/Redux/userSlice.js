import { createSlice } from '@reduxjs/toolkit';

const userSlice = createSlice({
  name: 'user',
  initialState: false, // Thay đổi initialState tùy thuộc vào cách bạn muốn lưu trạng thái đăng nhập
  reducers: {
    setLoggedIn: (state) => {
      return true;
    },
    setLoggedOut: (state) => {
      return false;
    },
  },
});

export const { setLoggedIn, setLoggedOut } = userSlice.actions;
export default userSlice.reducer;