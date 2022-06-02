/*
Ruta para la gestión de JSON ImgeItem
*/

const express = require('express');
const router = express.Router(); //Para crear rutas
const ImageItem = require('../models/ImageItem');
const multer = require('multer'); //para recibir las imagenes

const storage = multer.diskStorage({
    destination: function(req,file,cb){
        //Dirección donde almacenaremos las imagenes
        cb(null, './uploads/');
    },
    filename: function(req,file,cb){
        //Nombre con el que almacenaremos las imagenes
        cb(null, new Date().toISOString() + '_' + file.originalname);
    }
});

const upload = multer({storage: storage}); //carpeta donde se guardaran las imagenes

//ROUTES

//GET all available image items
router.get('/', async (req,res) => {
    try{
        //Retornar todos las imagenes de la DB
        const images = await ImageItem.find();
        res.json(images);
    }catch(err){
        res.json({message:err}); //devolver error en caso de fallo
    }
})

//GET all available image item's IDs
router.get('/IDs', async (req,res) => {
    try{
        //Retornar todos las ids de las imagenes de la DB
        const ids = await (await ImageItem.find({}, {projection: { _id: 1}})); // "projection" indica los campos que se incluirán en la respuesta
        res.json(ids);
    }catch(err){
        res.json({message:err}); //devolver error en caso de fallo
    }
})

//GET a specific image item
router.get('/:imageId', async (req,res) => {
    //imageId se añade en http://192.168.1.135:3000/posts/:imageId
    
    try{
        //Retornar imagen con determinada ID
        const image = await ImageItem.findById(req.params.imageId);
        res.json(image);
    }catch(err){
        res.json({message:err}); //devolver error en caso de fallo
    }
})

//POST an image item
router.post('/', upload.single('imageFile'), async (req,res) => {

    //Crear nuevo objeto ImageItem y rellenar con lo que recibimos
    const image = new ImageItem({
        imageFile: req.file.path,
        azimuth: req.body.azimuth,
        latitude: req.body.latitude,
        longitude: req.body.longitude
    });

    try{
    //Guardar en la DB
    const saveImage = await image.save();
    res.json(saveImage);
    }catch(err){
        res.json({message:err}); //devolver error en caso de fallo
    }
});

//DELETE a specific image item
router.delete('/:imageId', async (req,res) => {
    //imageId se añade en http://192.168.1.135:3000/posts/:imageId
    
    try{
        //Retornar imagen con determinada ID
        const removeImage = await ImageItem.deleteOne({_id: req.params.imageId}); //_id es generado por mongoDB, y debe coincidir con el imageId deseado
        res.json(removeImage);
    }catch(err){
        res.json({message:err}); //devolver error en caso de fallo
    }
})

//PATCH (update) a specific image item
router.patch('/:imageId', async (req,res) => {
    //imageId se añade en http://192.168.1.135:3000/posts/:imageId
    
    try{
        //Retornar imagen con determinada ID
        const updateImage = await ImageItem.updateOne(
            {_id: req.params.imageId}, //_id es generado por mongoDB, y debe coincidir con el imageId deseado
            {$set: {image64: req.body.image64}} //parametros que queremos actualizar
        );
        
        res.json(updateImage);

    }catch(err){
        res.json({message:err}); //devolver error en caso de fallo
    }
})

//Export router
module.exports = router;