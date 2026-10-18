package com.example.parcialkotlinapp
import org.junit.Assert.assertEquals
import org.junit.Test

class StudentLogicTest {
    @Test fun birthdayBoundaries() {
        assertEquals(19, StudentLogic.edad(2006, 10, 14, 2026, 10, 13))
        assertEquals(20, StudentLogic.edad(2006, 10, 13, 2026, 10, 13))
        assertEquals(20, StudentLogic.edad(2006, 10, 12, 2026, 10, 13))
        assertEquals(19, StudentLogic.edad(2006, 12, 1, 2026, 1, 1))
        assertEquals(20, StudentLogic.edad(2004, 2, 29, 2025, 2, 28))
        assertEquals(21, StudentLogic.edad(2004, 2, 29, 2025, 3, 1))
    }
    @Test fun greetingBoundaries() {
        for (hour in 0..23) assertEquals(when(hour) {
            in 5..11 -> "Buenos días"
            in 12..17 -> "Buenas tardes"
            else -> "Buenas noches"
        }, StudentLogic.saludo(hour))
    }
}
