package com.example.parcialkotlinapp

import android.app.DatePickerDialog
import android.os.Bundle
import android.widget.*
import androidx.appcompat.app.AlertDialog
import com.example.parcialkotlinapp.api.ApiClient
import com.example.parcialkotlinapp.api.ApiErrors
import com.example.parcialkotlinapp.models.CrearUsuarioRequest
import com.example.parcialkotlinapp.models.Usuario
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response
import java.util.Calendar
import java.util.Locale

class RegistroActivity : ScreenActivity() {
    private var fecha: Calendar? = null
    private var pending: Call<Usuario>? = null
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_registro)
        applyInsets()
        findViewById<ImageButton>(R.id.btnBack).setOnClickListener { finish() }
        val nombre = findViewById<EditText>(R.id.etNombre)
        val apellido = findViewById<EditText>(R.id.etApellido)
        val correo = findViewById<EditText>(R.id.etCorreoRegistro)
        val password = findViewById<EditText>(R.id.etPasswordRegistro)
        val universidad = findViewById<EditText>(R.id.etUniversidad)
        val fechaButton = findViewById<Button>(R.id.btnFechaNacimiento)
        val edadLabel = findViewById<TextView>(R.id.tvEdadCalculada)
        val semestre = findViewById<Spinner>(R.id.spSemestre)
        val registrar = findViewById<Button>(R.id.btnRegistrar)
        val mensaje = findViewById<TextView>(R.id.tvMensajeRegistro)
        semestre.adapter = ArrayAdapter(this, android.R.layout.simple_spinner_item,
            listOf("Seleccione semestre") + (1..10).map { "Semestre $it" }).apply {
            setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item)
        }
        fun mostrarFecha() {
            val selected = fecha ?: return
            fechaButton.text = String.format(Locale.ROOT, "%02d/%02d/%04d",
                selected.get(Calendar.DAY_OF_MONTH), selected.get(Calendar.MONTH) + 1, selected.get(Calendar.YEAR))
            val hoy = Calendar.getInstance()
            val age = StudentLogic.edad(selected.get(Calendar.YEAR), selected.get(Calendar.MONTH) + 1,
                selected.get(Calendar.DAY_OF_MONTH), hoy.get(Calendar.YEAR), hoy.get(Calendar.MONTH) + 1,
                hoy.get(Calendar.DAY_OF_MONTH))
            edadLabel.text = "Edad calculada: $age años"
        }
        if (savedInstanceState?.containsKey("fecha") == true) {
            fecha = Calendar.getInstance().apply { timeInMillis = savedInstanceState.getLong("fecha") }
            mostrarFecha()
        }
        fechaButton.setOnClickListener {
            val initial = fecha ?: Calendar.getInstance().apply { add(Calendar.YEAR, -18) }
            val dialog = DatePickerDialog(this, { _, y, m, d ->
                fecha = Calendar.getInstance().apply { clear(); set(y, m, d) }
                mostrarFecha()
            }, initial.get(Calendar.YEAR), initial.get(Calendar.MONTH), initial.get(Calendar.DAY_OF_MONTH))
            dialog.datePicker.maxDate = System.currentTimeMillis()
            dialog.show()
        }
        registrar.setOnClickListener {
            mensaje.text = ""
            val name = nombre.text.toString().trim()
            val last = apellido.text.toString().trim()
            val email = correo.text.toString().trim()
            val pass = password.text.toString()
            val uni = universidad.text.toString().trim()
            if (name.isBlank() || name.length > 100) { nombre.error = "Nombre obligatorio, máximo 100 caracteres"; return@setOnClickListener }
            if (last.isBlank() || last.length > 100) { apellido.error = "Apellido obligatorio, máximo 100 caracteres"; return@setOnClickListener }
            if (email.length > 150 || !android.util.Patterns.EMAIL_ADDRESS.matcher(email).matches()) {
                correo.error = "Ingrese un correo válido"; return@setOnClickListener
            }
            if (pass.isBlank() || pass.length < 6 || pass.toByteArray(Charsets.UTF_8).size > 72) {
                password.error = "Contraseña de al menos 6 caracteres y máximo 72 bytes UTF-8"; return@setOnClickListener
            }
            val selected = fecha
            if (selected == null) { mensaje.text = "Seleccione la fecha de nacimiento"; return@setOnClickListener }
            val hoy = Calendar.getInstance()
            val age = StudentLogic.edad(selected.get(Calendar.YEAR), selected.get(Calendar.MONTH) + 1,
                selected.get(Calendar.DAY_OF_MONTH), hoy.get(Calendar.YEAR), hoy.get(Calendar.MONTH) + 1,
                hoy.get(Calendar.DAY_OF_MONTH))
            if (selected.after(hoy) || age !in 0..120) { mensaje.text = "Fecha de nacimiento no válida"; return@setOnClickListener }
            if (uni.isBlank() || uni.length > 150) { universidad.error = "Universidad obligatoria, máximo 150 caracteres"; return@setOnClickListener }
            if (semestre.selectedItemPosition !in 1..10) { mensaje.text = "Seleccione un semestre entre 1 y 10"; return@setOnClickListener }
            val iso = String.format(Locale.ROOT, "%04d-%02d-%02d", selected.get(Calendar.YEAR),
                selected.get(Calendar.MONTH) + 1, selected.get(Calendar.DAY_OF_MONTH))
            registrar.isEnabled = false
            mensaje.text = "Registrando usuario..."
            pending = ApiClient.usuarioApi.crearUsuario(CrearUsuarioRequest(name, last, email, pass, iso, uni, semestre.selectedItemPosition))
            pending!!.enqueue(object : Callback<Usuario> {
                override fun onResponse(call: Call<Usuario>, response: Response<Usuario>) {
                    if (isFinishing || isDestroyed) return
                    registrar.isEnabled = true
                    if (response.isSuccessful && response.body() != null) {
                        registrar.isEnabled = false
                        password.text.clear()
                        mensaje.text = "Usuario registrado correctamente"
                        AlertDialog.Builder(this@RegistroActivity).setTitle("Registro exitoso")
                            .setMessage("Su cuenta fue creada. Inicie sesión con el correo y la contraseña registrados.")
                            .setCancelable(false).setPositiveButton("ACEPTAR") { _, _ -> returnToLogin() }.show()
                    } else mensaje.text = if (response.isSuccessful) "Respuesta de registro incompleta." else ApiErrors.message(response)
                }
                override fun onFailure(call: Call<Usuario>, t: Throwable) {
                    if (call.isCanceled || isFinishing || isDestroyed) return
                    registrar.isEnabled = true
                    mensaje.text = "No se pudo conectar con el servidor. Revise la conexión y vuelva a intentar."
                }
            })
        }
    }
    override fun onSaveInstanceState(outState: Bundle) {
        fecha?.let { outState.putLong("fecha", it.timeInMillis) }
        super.onSaveInstanceState(outState)
    }
    override fun onDestroy() { pending?.cancel(); super.onDestroy() }
}
