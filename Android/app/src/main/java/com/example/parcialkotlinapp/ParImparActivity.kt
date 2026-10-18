package com.example.parcialkotlinapp

import android.os.Bundle
import android.widget.Button
import android.widget.EditText
import android.widget.ImageButton
import android.widget.TextView

class ParImparActivity : ScreenActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        if (!requireSession()) return
        setContentView(R.layout.activity_par_impar)
        applyInsets()
        val numero = findViewById<EditText>(R.id.etNumero)
        val resultado = findViewById<TextView>(R.id.tvResultado)
        findViewById<Button>(R.id.btnValidar).setOnClickListener {
            val value = numero.text.toString().trim().toIntOrNull()
            if (value == null) { numero.error = "Ingrese un número entero válido"; return@setOnClickListener }
            resultado.text = if (value % 2 == 0) "$value es PAR" else "$value es IMPAR"
        }
        findViewById<ImageButton>(R.id.btnBack).setOnClickListener { finish() }
    }
    override fun onResume() { super.onResume(); requireSession() }
}
