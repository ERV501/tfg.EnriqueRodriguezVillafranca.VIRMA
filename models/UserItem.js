const mongoose = require('mongoose');
const bcrypt = require('bcrypt');

const UserItemSchema = mongoose.Schema({
    username: {
        type: String,
        required: true,
        unique: true
    },
    password: {
        type: String,
        required: true
    },
    timestamp: {
        type: Date,
        default: Date.now
    },
});

//Cifrar password de usuario
UserItemSchema.statics.encryptPassword = async (password) => {
    const salt = await bcrypt.genSalt(); //generar salto (por defecto 10 bloques)
    return await bcrypt.hash(password, salt); //cifrar password
}

//Comparar password de usuario ya existente
UserItemSchema.statics.checkPassword = async (password, newPassword) => {
    return await bcrypt.compare(password, newPassword); //retornar si coinciden o no
}

module.exports = mongoose.model('UserItems', UserItemSchema); //Nombre y esquema