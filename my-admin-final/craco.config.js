module.exports = {
  webpack: {
    configure: (webpackConfig) => {
        // Thêm cấu hình webpack ở đây (resolve.fallback) cho module 'buffer'
        webpackConfig.resolve.fallback = { "buffer": require.resolve("buffer/") };

        // Thêm cấu hình webpack ở đây (resolve.fallback) cho module 'crypto'
          webpackConfig.resolve.fallback = {
              ...webpackConfig.resolve.fallback,
              "process": require.resolve('process/browser'),
              "crypto": require.resolve("crypto-browserify"),
              "util": require.resolve("util/"),
                "stream": require.resolve("stream-browserify")
          }; 
      return webpackConfig;
    },
  },
};
