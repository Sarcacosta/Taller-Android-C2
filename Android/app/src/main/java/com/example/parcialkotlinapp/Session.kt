package com.example.parcialkotlinapp

import com.example.parcialkotlinapp.models.Usuario

// Sesión local en memoria. Nunca guarda contraseña ni hash.
object Session {
    var usuario: Usuario? = null
    fun clear() { usuario = null }
}
