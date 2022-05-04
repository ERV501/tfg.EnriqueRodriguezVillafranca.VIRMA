/*
Ruta para la auteticación de usuarios
*/

const express = require('express');
const router = express.Router(); //Para crear rutas
const UserItem = require('../models/UserItem');

//ROUTES

//GET all available users
router.get('/', async (req,res) => {
    try{
        //Retornar todos las users de la DB
        const users = await UserItem.find();
        res.json(users);
    }catch(err){
        res.json({message:err}); //devolver error en caso de fallo
    }
});

//POST para registro de ususario
router.post('/register', async (req,res) => {
    
    console.log("Registering User");

    //Crear nuevo objeto User y rellenar con lo que recibimos
    const user = new UserItem({
        username: req.body.username,
        password: await UserItem.encryptPassword(req.body.password)
    });

    console.log(user);

    try{
    //Guardar en la DB
    const registerUser = await user.save();
    res.json(registerUser);
    }catch(err){
        res.json({message:err}); //devolver error en caso de fallo
    }
});

//POST para login de ususario
router.post('/login', async (req,res) => {

    //Crear nuevo objeto User y rellenar con lo que recibimos
    const user = new UserItem({
        username: req.body.username,
        password: req.body.password
    });

    try{
    //Guardar en la DB
    const loginUser = await user.save();
    res.json(loginUser);
    }catch(err){
        res.json({message:err}); //devolver error en caso de fallo
    }
});

//Export router
module.exports = router;