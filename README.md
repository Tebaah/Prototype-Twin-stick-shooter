# Arena Cero

Prototipo de **twin-stick shooter 2D** desarrollado con **Godot 4.7** y **C#**. El jugador sobrevive en una arena de 12x12, elimina enemigos y avanza por niveles.

## Requisitos

- Godot 4.7 .NET
- .NET SDK 8.0
- Windows o Linux

## Ejecución

1. Abrir `Prototype-Twin-stick-shooter.sln` o el proyecto desde Godot.
2. Compilar con `dotnet build`.
3. Ejecutar la escena principal con **F5**.

La escena jugable es `Scenes/main.tscn`. `Scenes/Testing/map_testing.tscn` queda como escena de pruebas.

## Controles

| Acción | Entrada |
| --- | --- |
| Moverse | WASD |
| Apuntar | Ratón |
| Disparar | Click izquierdo; mantener para 5 disparos/s |
| Reiniciar al morir | R |
| Salir | ESC |

## Mecánicas

- El jugador comienza con **3 vidas**. Los corazones se ocultan al recibir daño.
- **Rusher**: persigue al jugador y explota al contacto, sin otorgar puntaje.
- **Shooter/Sniper**: mantiene una distancia de 130–170 px y dispara cada **1.5 s**.
- Los enemigos otorgan entre **10 y 50 puntos** cuando mueren por una bala del jugador.
- Al eliminar todos los enemigos aparece **LEVEL CLEAR** durante un segundo y se carga el siguiente nivel.
- Al quedarse sin vidas aparece **GAME OVER**; el juego se pausa y R reinicia la partida.
- Los obstáculos se generan proceduralmente, bloquean al jugador y a las balas, permiten el paso de enemigos y evitan bloques 3x3.
- El `LevelManager` define la composición de los niveles 1 a 10.

## Estructura

```text
Scenes/
  main.tscn              Escena principal
  Player/                Jugador, estados y ataque
  Enemy/                 Rusher, Shooter y estados de IA
  Bullet/                Balas del jugador y enemigas
  Components/            Health, Hitbox y Detection
  UI/                    HUD
  Map/ y Obstacles/      Arena y obstáculos procedurales
  Testing/               Escenas de prueba
Scripts/
  Managers/              GameManager y LevelManager
  Components/            Componentes reutilizables
  Player/                Movimiento, apuntado y ataque
  Enemy/                 Controladores y máquinas de estado
  Bullet/                Proyectiles
  UI/                    HUD
```

## Componentes principales

- `HealthComponent`: salud máxima, daño, curación y muerte.
- `HitboxComponent`: detección de jugador, enemigos, balas y obstáculos.
- `DetectionComponent`: detección del jugador con radio configurable.
- `EnemyController`: base común de Rusher y Shooter.
- `GameManager`: vidas, nivel, enemigos, puntaje, Game Over y Level Clear.
- `LevelManager`: composición de enemigos y obstáculos por nivel.

## Documentación y estado

El diseño completo está en [`GDD - Arena Cero.md`](GDD%20-%20Arena%20Cero.md).

**Estado:** prototipo v1.0 validado.

Limitaciones de la v1.0: sin audio, menús, guardado, power-ups, pathfinding avanzado, múltiples armas ni récords persistentes.

## Verificación rápida

```bash
dotnet build
godot-mono --headless --path . --quit-after 120
```
