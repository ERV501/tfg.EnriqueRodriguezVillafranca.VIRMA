const express = require('express'); //facilita la creacion del servidor http
const app = express(); //ejecutar express
const mongoose = require('mongoose');
const bodyParser = require('body-parser'); //para parsear JSON

//MIDDLEWARES
app.use(bodyParser.json()); //usar json body parser para cualquier peticion

//Import routes
const imagesRoute = require('./routes/images');
app.use('/images', imagesRoute);

const usersRoute = require('./routes/users');
app.use('/users', usersRoute);

//ROUTES
app.get('/',(req,res) => {
    res.send('Server is up');
})

//Connect to DB
mongoose.connect(
    'mongodb+srv://erv:erv@rest.kjphb.mongodb.net/test'
    ,{useNewUrlParser: true}
    ,() => console.log('Connected to DB')
);

//Listen to the server
app.listen(3000);