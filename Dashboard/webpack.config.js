const path = require('path');
var HtmlWebpackPlugin = require("html-webpack-plugin");
var HTMLWebpackPluginConfig = new HtmlWebpackPlugin({
  template: __dirname + '/src/index.html',
  filename: 'index.html',
  inject: false
});

module.exports = {
  entry:  path.join(__dirname, './src/index.js'),
  output: {
    path: path.join(__dirname, 'dist'),
    filename: 'build.js'
  },
  plugins: [HTMLWebpackPluginConfig],
  devServer: {
    inline: true,
    compress: true,
    port: 3333
  },
  module: {
    rules: [
      {
        test: /\.js?$/,
        exclude: /(node_modules)/,
        loader: "babel-loader",
        options: {
          cacheDirectory: true
        }
      },
      {
        test: /\.html$/,
        use: [
          {
            loader: "html-loader"
          }
        ]
      }
    ]
  }
};