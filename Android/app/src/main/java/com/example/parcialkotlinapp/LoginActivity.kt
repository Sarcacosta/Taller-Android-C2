package com.example.parcialkotlinapp

import android.content.Intent
import android.os.Bundle
import android.widget.Button
import android.widget.EditText
import android.widget.TextView
import com.example.parcialkotlinapp.api.ApiClient
import com.example.parcialkotlinapp.api.ApiErrors
import com.example.parcialkotlinapp.models.LoginRequest
import com.example.parcialkotlinapp.models.Usuario
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response

class LoginActivity : ScreenActivity() {
    private var pending: Call<Usuario>? = null
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_login)
        applyInsets()
        val correo = findViewById<EditText>(R.id.etCorreo)
        val password = findViewById<EditText>(R.id.etPassword)
        val ingresar = findViewById<Button>(R.id.btnIngresar)
        val crear = findViewById<Button>(R.id.btnCrearCuenta)
        val mensaje = findViewById<TextView>(R.id.tvMensaje)
        ingresar.setOnClickListener {
            val email = correo.text.toString().trim()
            val pass = password.text.toString()
            if (!android.util.Patterns.EMAIL_ADDRESS.matcher(email).matches()) {
                correo.error = "Ingrese un correo válido"; return@setOnClickListener
            }
            if (pass.isBlank()) { password.error = "Ingrese la contraseña"; return@setOnClickListener }
            ingresar.isEnabled = false
            crear.isEnabled = false
            mensaje.text = "Validando usuario..."
            pending = ApiClient.usuarioApi.login(LoginRequest(email, pass))
            pending!!.enqueue(object : Callback<Usuario> {
                override fun onResponse(call: Call<Usuario>, response: Response<Usuario>) {
                    if (isFinishing || isDestroyed) return
                    ingresar.isEnabled = true
                    crear.isEnabled = true
                    val usuario = response.body()
                    if (response.isSuccessful && usuario != null) {
                        Session.usuario = usuario
                        password.text.clear()
                        startActivity(Intent(this@LoginActivity, MainActivity::class.java).apply {
                            flags = Intent.FLAG_ACTIVITY_NEW_TASK or Intent.FLAG_ACTIVITY_CLEAR_TASK
                        })
                        finish()
                    } else mensaje.text = if (response.isSuccessful) "Respuesta de Login incompleta." else ApiErrors.message(response)
                }
                override fun onFailure(call: Call<Usuario>, t: Throwable) {
                    if (call.isCanceled || isFinishing || isDestroyed) return
                    ingresar.isEnabled = true
                    crear.isEnabled = true
                    mensaje.text = "No se pudo conectar con el servidor. Revise la conexión."
                }
            })
        }
        crear.setOnClickListener { startActivity(Intent(this, RegistroActivity::class.java)) }
    }
    override fun onDestroy() { pending?.cancel(); super.onDestroy() }
}
