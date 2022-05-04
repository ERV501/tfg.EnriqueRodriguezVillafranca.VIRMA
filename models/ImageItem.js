const mongoose = require('mongoose');

const ImageItemSchema = mongoose.Schema({
    imageFile: {
        type: String,
        required: true
    },
    azimuth: {
        type: Number,
        required: true
    },
    latitude: {
        type: Number,
        required: true
    },
    longitude: {
        type: Number,
        required: true
    },
    timestamp: {
        type: Date,
        default: Date.now
    }
});

module.exports = mongoose.model('ImageItem', ImageItemSchema); //Nombre y esquema