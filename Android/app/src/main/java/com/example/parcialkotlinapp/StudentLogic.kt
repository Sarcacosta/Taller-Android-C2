package com.example.parcialkotlinapp

object StudentLogic {
    fun edad(year: Int, month: Int, day: Int, todayYear: Int, todayMonth: Int, todayDay: Int): Int {
        var age = todayYear - year
        if (todayMonth < month || (todayMonth == month && todayDay < day)) age--
        return age
    }
    fun saludo(hour: Int): String = when (hour) {
        in 5..11 -> "Buenos días"
        in 12..17 -> "Buenas tardes"
        else -> "Buenas noches"
    }
}
