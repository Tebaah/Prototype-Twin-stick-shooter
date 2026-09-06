# GDD - Arena Cero

***

## INFORMACIÓN GENERAL

| Campo          | Valor                                   |
| -------------- | --------------------------------------- |
| **Título**     | Arena Cero                              |
| **Versión**    | 1.0                                     |
| **Fecha**      | 06/09/2026                              |
| **Plataforma** | PC (Windows / Linux)                    |
| **Motor**      | Godot (C#)                              |
| **Género**     | Twin-stick shooter                      |
| **Público**    | Desarrollador independiente / Portfolio |
| **Estado**     | En diseño                               |

***

## 1. CONCEPTO CENTRAL

Arena Cero es un twin-stick shooter minimalista para un jugador. El jugador controla un personaje en una arena cuadriculada de 12x12, enfrentándose a oleadas de enemigos mientras esquiva obstáculos generados proceduralmente. El juego está diseñado para ser completado en su versión 1.0 sin ambiciones excesivas, priorizando la jugabilidad funcional sobre el contenido extenso.

### Filosofía de diseño

- **Simplicidad**: Mecánicas claras y directas
- **Terminable**: Alcance realista para un desarrollador solitario
- **Progresión**: Dificultad creciente sin cambios mecánicos
- **Portfolio**: Demostración de habilidades técnicas y de diseño

***

## 2. MECÁNICAS PRINCIPALES

### 2.1 Jugador

| Característica           | Especificación            |
| ------------------------ | ------------------------- |
| **Movimiento**           | Teclas WASD               |
| **Apuntado**             | Cursor del ratón          |
| **Disparo**              | Click izquierdo del ratón |
| **Velocidad movimiento** | 200 píxeles/segundo       |
| **Tamaño**               | 32x32 píxeles             |
| **Vida inicial**         | 3 puntos                  |
| **Cadencia disparo**     | 5 disparos/segundo        |

### 2.1.1 Diagramas

#### Flujo de movimiento
![Fujo de movimiento](https://i.ibb.co/ksXCyLsn/Persona-movimiento.png)
#### Flujo de ataque
![Flujo de ataque](https://i.ibb.co/fjcVhn7/Personaje-ataque.png)
#### Flujo de vida
![Flujo vida](https://i.ibb.co/8n64xmZJ/Personaje-vida.png)


### 2.2 Enemigos

#### Tipo A: Rusher

| Característica     | Especificación                   |
| ------------------ | -------------------------------- |
| **Color**          | Rojo brillante                   |
| **Tamaño**         | 32x32 píxeles                    |
| **Comportamiento** | Persigue directamente al jugador |
| **Velocidad**      | Alta (150% velocidad jugador)    |
| **Vida**           | 1 golpe                          |
| **Daño**           | -1 vida al contacto (explota)    |

#### Tipo B: Sniper

| Característica     | Especificación                            |
| ------------------ | ----------------------------------------- |
| **Color**          | Naranja                                   |
| **Tamaño**         | 32x32 píxeles                             |
| **Comportamiento** | Se aleja del jugador, dispara proyectiles |
| **Velocidad**      | Baja (50% velocidad jugador)              |
| **Vida**           | 2 golpes                                  |
| **Daño**           | -1 vida por proyectil                     |
| **Cadencia**       | 1 disparo cada 1.5 segundos               |

### 2.2.1 Diagramas
#### Flujo ataque Rusher
![Flujo de ataque](https://i.ibb.co/8DhwWV37/Enemigo-Rusher.png)
#### Flujo ataque Sniper
![Flujo de ataque](https://i.ibb.co/G3RF1fvd/Enemigo-Sniper.png)

### 2.3 Obstáculos

| Característica     | Especificación                                            |
| ------------------ | --------------------------------------------------------- |
| **Cantidad**       | 29 (20% de 144 celdas)                                    |
| **Tamaño**         | 64x64 píxeles                                             |
| **Comportamiento** | Sólidos para el jugador, atravesables por enemigos (v1.0) |
| **Generación**     | Aleatoria con zona segura y sin bloques 3x3               |

### 2.4 Escenario

| Característica   | Especificación                                        |
| ---------------- | ----------------------------------------------------- |
| **Dimensiones**  | 768 x 768 píxeles                                     |
| **Grid**         | 12x12 celdas                                          |
| **Tamaño celda** | 64 x 64 píxeles                                       |
| **Zona segura**  | Celdas (5,5), (5,6), (6,5), (6,6) - Spawn del jugador |

***

## 3. PROGRESIÓN DE NIVELES

### 3.1 Composición por nivel

| Nivel | Rushers | Snipers | Obstáculos | Dificultad |
| ----- | ------- | ------- | ---------- | ---------- |
| 1     | 3       | 2       | 15 (10%)   | Muy fácil  |
| 2     | 3       | 2       | 15         | Muy fácil  |
| 3     | 4       | 2       | 15         | Fácil      |
| 4     | 4       | 3       | 18 (13%)   | Fácil      |
| 5     | 5       | 3       | 18         | Moderada   |
| 6     | 5       | 3       | 18         | Moderada   |
| 7     | 6       | 4       | 22 (15%)   | Desafiante |
| 8     | 6       | 4       | 22         | Desafiante |
| 9     | 7       | 4       | 22         | Difícil    |
| 10    | 7       | 5       | 29 (20%)   | Difícil    |

### 3.2 Reglas de progresión

- Al eliminar todos los enemigos → "LEVEL CLEAR" (1 segundo)
- Regenerar mapa con nuevos obstáculos
- Aumentar número de enemigos según tabla
- El jugador recupera 1 vida por nivel completado (opcional - decisión pendiente)

### 3.3 Condiciones de fin

| Evento               | Acción                                |
| -------------------- | ------------------------------------- |
| **Vida jugador = 0** | Game Over - Detener todo              |
| **Enemigos = 0**     | Level Clear - Generar siguiente nivel |

***

## 4. ESTRUCTURA DE DATOS

### Grid (Matriz 12x12)

### Representación visual

```javascript
    0  1  2  3  4  5  6  7  8  9 10 11
   ------------------------------------
 11| .  .  .  █  .  .  .  .  █  .  .  .
 10| .  █  .  .  .  .  .  .  .  .  █  .
  9| .  .  .  .  █  .  .  █  .  .  .  .
  8| █  .  .  .  .  .  .  .  .  █  .  .
  7| .  .  .  .  .  █  .  .  .  .  .  █
  6| .  .  █  .  .  ░  ░  .  .  .  .  .
  5| .  .  .  .  .  ░  ░  █  .  .  .  .
  4| .  .  .  █  .  .  .  .  █  .  .  .
  3| .  █  .  .  .  .  .  .  .  .  .  █
  2| █  .  .  .  .  .  █  .  .  █  .  .
  1| .  .  .  █  .  .  .  .  .  .  .  .
  0| .  .  █  .  █  .  .  .  .  .  █  .
   ------------------------------------
  █ = Obstáculo (29) | ░ = Zona segura (4) | · = Espacio libre
```

***

## 5. INTERFAZ DE USUARIO

### HUD (v1.0)

| Elemento               | Posición                   | Descripción                  |
| ---------------------- | -------------------------- | ---------------------------- |
| **Vida**               | Esquina superior izquierda | Corazones o número (ej: ♥♥♥) |
| **Nivel**              | Centro superior            | "NIVEL 3"                    |
| **Enemigos restantes** | Esquina superior derecha   | "3 / 10"                     |
| **Game Over**          | Centro de pantalla         | Texto rojo, reinicio con R   |
| **Level Clear**        | Centro de pantalla         | Texto verde, 1 segundo       |
### Diseños
![HUD](https://i.ibb.co/RkXBR914/Interfaz-grafica.png)
![Game Over](https://i.ibb.co/kF0jDw4/Interfaz-game-over.png)
![Level Clear](https://i.ibb.co/r2HvGWFS/Interfaz-level-clear.png)

### Controles

| Acción                | Tecla              |
| --------------------- | ------------------ |
| Moverse               | W A S D            |
| Apuntar               | Ratón (movimiento) |
| Disparar              | Click izquierdo    |
| Reiniciar (Game Over) | R                  |
| Salir                 | ESC                |

***

## 6. RESTRICCIONES Y LIMITACIONES (VERSIÓN 1.0)

### NO INCLUIR en v1.0

- ❌ Efectos de sonido
- ❌ Animaciones (excepto movimiento básico)
- ❌ Menús (inicio / opciones)
- ❌ Power-ups o mejoras
- ❌ Puntuación o récords
- ❌ Guardado de partida
- ❌ Múltiples armas o tipos de disparo
- ❌ Enemigos con IA compleja (pathfinding)
- ❌ Diferentes tamaños de mapa
- ❌ Música o efectos ambientales
- ❌ Tutorial o instrucciones en pantalla

### PERMITIDO si es SIMPLE

- ✅ Diferentes colores para distinguir elementos
- ✅ Contador de vidas (texto simple)
- ✅ Indicador de nivel actual
- ✅ Reinicio con tecla R
- ✅ Texto de Game Over / Level Clear

***

## 7. CRITERIO DE "TERMINADO"

El juego se considera terminado cuando se cumple **TODOS** estos puntos:

1. ✅ El jugador se mueve con WASD
2. ✅ El jugador apunta con el ratón
3. ✅ El jugador dispara con click izquierdo
4. ✅ Los enemigos Rusher persiguen al jugador
5. ✅ Los enemigos Sniper disparan proyectiles
6. ✅ El jugador pierde vida al tocar enemigos o proyectiles
7. ✅ Los obstáculos bloquean al jugador y las balas
8. ✅ Se generan 29 obstáculos con zona segura
9. ✅ Se pueden jugar al menos 3 niveles consecutivos
10. ✅ Game Over al llegar a 0 vidas
11. ✅ Reinicio del juego con tecla R
12. ✅ No hay crashes o errores críticos

***

## 8. MÉTRICAS DE ÉXITO

| Métrica                  | Objetivo                   |
| ------------------------ | -------------------------- |
| **Tiempo de desarrollo** | 10-13 días (2-3 horas/día) |
| **Líneas de código**     | < 500 por script principal |
| **FPS**                  | 60 estables                |
| **Tamaño build**         | < 50 MB                    |
| **Niveles jugables**     | Mínimo 3 consecutivos      |
