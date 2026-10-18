package com.example.parcialkotlinapp.api

import com.example.parcialkotlinapp.models.CrearUsuarioRequest
import com.example.parcialkotlinapp.models.LoginRequest
import com.example.parcialkotlinapp.models.Usuario
import retrofit2.Call
import retrofit2.http.Body
import retrofit2.http.GET
import retrofit2.http.POST

interface UsuarioApi {

    // Obtener todos los usuarios
    @GET("api/Usuarios")
    fun obtenerUsuarios(): Call<List<Usuario>>

    // Crear usuario
    @POST("api/Usuarios")
    fun crearUsuario(
        @Body usuario: CrearUsuarioRequest
    ): Call<Usuario>

    // Login
    @POST("api/Usuarios/login")
    fun login(
        @Body login: LoginRequest
    ): Call<Usuario>
}