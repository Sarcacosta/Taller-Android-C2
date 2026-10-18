package com.example.parcialkotlinapp

import android.content.Intent
import android.os.Bundle
import android.widget.Button
import android.widget.ImageButton
import android.widget.TextView
import androidx.appcompat.app.AlertDialog
import java.util.Calendar

class MainActivity : ScreenActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        if (!requireSession()) return
        setContentView(R.layout.activity_main)
        applyInsets()
        findViewById<Button>(R.id.btnParImpar).setOnClickListener {
            startActivity(Intent(this, ParImparActivity::class.java))
        }
        findViewById<ImageButton>(R.id.btnLogout).setOnClickListener {
            AlertDialog.Builder(this).setTitle("Cerrar sesión")
                .setMessage("¿Desea salir de su cuenta?")
                .setNegativeButton("CANCELAR") { dialog, _ -> dialog.dismiss() }
                .setPositiveButton("SALIR") { _, _ -> Session.clear(); returnToLogin() }.show()
        }
    }
    override fun onResume() {
        super.onResume()
        if (!requireSession()) return
        val nombre = Session.usuario!!.nombre
        val saludo = StudentLogic.saludo(Calendar.getInstance().get(Calendar.HOUR_OF_DAY))
        findViewById<TextView>(R.id.tvSaludo).text = "$saludo, $nombre"
    }
}
