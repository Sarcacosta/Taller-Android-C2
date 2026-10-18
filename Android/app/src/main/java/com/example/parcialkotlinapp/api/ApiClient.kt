package com.example.parcialkotlinapp.api

import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory

object ApiClient {

    // 10.0.2.2 representa nuestro PC desde el emulador Android
    private const val BASE_URL =
        "http://10.0.2.2:62129/"

    private val retrofit: Retrofit =
        Retrofit.Builder()
            .baseUrl(BASE_URL)
            .addConverterFactory(
                GsonConverterFactory.create()
            )
            .build()

    val usuarioApi: UsuarioApi =
        retrofit.create(UsuarioApi::class.java)
}