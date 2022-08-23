var path = require('path');
var HtmlWebpackPlugin = require('html-webpack-plugin');


function resolve(filePath) {
    return path.isAbsolute(filePath) ? filePath : path.join(__dirname, filePath);
}

var cfg = {
    projectPath: './Demo/',
    index: './Demo/index.html',
    entry: './fable_output/Main.js',
    output: './dist'
}

// The HtmlWebpackPlugin allows us to use a template for the index.html page
// and automatically injects <script> or <link> tags for generated bundles.
var htmlPlugin =
    new HtmlWebpackPlugin({
        filename: 'index.html',
        template: resolve(cfg.index)
    });

// Configuration for webpack-dev-server
var devServer = {
    host: 'localhost',
    port: 8088,
    hot: true
};

// If we're running the webpack-dev-server, assume we're in development mode
var isProduction = !process.argv.find(v => v.indexOf('webpack-dev-server') !== -1);
var environment = isProduction ? 'production' : 'development';
process.env.NODE_ENV = environment;
console.log('Bundling for ' + environment + '...');

module.exports = {
    entry: { app: resolve(cfg.entry) },
    output: {
        path: resolve(cfg.output),
        filename: isProduction ? '[name].[contenthash].js' : '[name].js',
        clean: true
    },
    devtool: isProduction ? undefined : 'eval-source-map',
    resolve: {
        alias: {
            Project: path.resolve(__dirname, cfg.projectPath)
        },
        symlinks: false
    }, // See https://github.com/fable-compiler/Fable/issues/1490
    mode: environment,
    plugins: [htmlPlugin],
    optimization: { splitChunks: { chunks: 'all' } },
    devServer: devServer,
    module: {
        rules: [
            {
                test: /\.js$/,
                enforce: "pre",
                exclude: /node_modules|fable_modules/,
                use: ["source-map-loader"]
            }
        ]
    }
};