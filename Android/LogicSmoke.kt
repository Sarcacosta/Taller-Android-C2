import com.example.parcialkotlinapp.StudentLogic
fun main() {
    check(StudentLogic.edad(2006, 10, 14, 2026, 10, 13) == 19)
    check(StudentLogic.edad(2006, 10, 13, 2026, 10, 13) == 20)
    check(StudentLogic.edad(2006, 10, 12, 2026, 10, 13) == 20)
    check(StudentLogic.edad(2004, 2, 29, 2025, 2, 28) == 20)
    check(StudentLogic.edad(2004, 2, 29, 2025, 3, 1) == 21)
    for (hour in 0..23) check(StudentLogic.saludo(hour) == when(hour) {
        in 5..11 -> "Buenos días"
        in 12..17 -> "Buenas tardes"
        else -> "Buenas noches"
    })
    println("29 comprobaciones Kotlin correctas de edad y saludo")
}
