package com.example.parcialkotlinapp.api

import org.json.JSONObject
import retrofit2.Response

object ApiErrors {
    fun message(response: Response<*>): String {
        val raw = response.errorBody()?.string()
        return parse(response.code(), raw)
    }
    fun parse(code: Int, raw: String?): String {
        try {
            val json = JSONObject(raw.orEmpty())
            val errors = json.optJSONObject("errors")
            if (errors != null) {
                val details = mutableListOf<String>()
                val keys = errors.keys()
                while (keys.hasNext()) {
                    val value = errors.optJSONArray(keys.next()) ?: continue
                    for (i in 0 until value.length()) details.add(value.optString(i))
                }
                if (details.isNotEmpty()) return details.joinToString("\n")
            }
            val message = json.optString("mensaje")
            if (message.isNotBlank()) return message
        } catch (_: Exception) { /* Respuesta vacía o no JSON */ }
        return when (code) {
            400 -> "Revise los datos enviados (HTTP 400)."
            401 -> "Correo o contraseña incorrectos."
            in 500..599 -> "El servidor no pudo procesar la solicitud (HTTP $code)."
            else -> "No se pudo completar la solicitud (HTTP $code)."
        }
    }
}
