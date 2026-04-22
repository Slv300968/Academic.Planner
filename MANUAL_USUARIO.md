# Manual de Usuario — Academic Planner
### Preparación para el Examen UNAM Área 2

---

## Índice

1. [Introducción](#1-introducción)
2. [Navegación general](#2-navegación-general)
3. [Dashboard — Panel de Control](#3-dashboard--panel-de-control)
4. [Materias](#4-materias)
5. [Detalle de materia y temas](#5-detalle-de-materia-y-temas)
   - 5.1 [Cambiar estado de un tema](#51-cambiar-estado-de-un-tema)
   - 5.2 [Apuntes del tema](#52-apuntes-del-tema)
   - 5.3 [Material de estudio](#53-material-de-estudio)
   - 5.4 [Preguntas del tema](#54-preguntas-del-tema)
6. [Flashcards](#6-flashcards)
   - 6.1 [Ver y voltear tarjetas](#61-ver-y-voltear-tarjetas)
   - 6.2 [Crear una tarjeta nueva](#62-crear-una-tarjeta-nueva)
   - 6.3 [Fórmulas LaTeX](#63-fórmulas-latex)
   - 6.4 [Agregar gráfica a una tarjeta](#64-agregar-gráfica-a-una-tarjeta)
7. [Simulador de Examen](#7-simulador-de-examen)
8. [Planner de Estudio](#8-planner-de-estudio)
   - 8.1 [Configurar el plan](#81-configurar-el-plan)
   - 8.2 [Vista Cronograma (Gantt)](#82-vista-cronograma-gantt)
   - 8.3 [Vista Tablero](#83-vista-tablero)
   - 8.4 [Vista Calendario](#84-vista-calendario)
9. [Estados de avance](#9-estados-de-avance)
10. [Flujo de trabajo recomendado](#10-flujo-de-trabajo-recomendado)

---

## 1. Introducción

**Academic Planner** es una herramienta de estudio diseñada para ayudarte a preparar el **Examen de Admisión UNAM Área 2**. La aplicación cubre las **9 materias del temario oficial CENEVAL**:

| Materia | Materia |
|---|---|
| Español | Literatura |
| Matemáticas | Biología |
| Química | Física |
| Historia de México | Historia Universal |
| Geografía | |

Con Academic Planner puedes:
- Hacer seguimiento de qué temas ya **dominas**, cuáles tienes **en progreso** y cuáles están **pendientes**.
- Tomar **apuntes** directamente en cada tema y agregar **recursos de estudio** (videos, PDFs, artículos).
- Crear y repasar **flashcards** con soporte para texto enriquecido, fórmulas matemáticas y gráficas.
- Practicar con el **simulador de examen** que genera reactivos de forma aleatoria.
- Generar un **plan de estudio personalizado** con distribución por fechas y horas disponibles.

---

## 2. Navegación general

La barra lateral izquierda está siempre visible y contiene los accesos principales:

| Ícono | Sección | Descripción |
|---|---|---|
| 🏠 | **Dashboard** | Resumen global de tu avance |
| 📖 | **Materias** | Lista de todas las materias con progreso |
| 🎯 | **Simulador** | Simulacro de examen con reactivos aleatorios |
| 📅 | **Planner** | Generador de plan de estudio con calendario |

Haz clic en cualquier ítem del menú para navegar entre secciones.

---

## 3. Dashboard — Panel de Control

La pantalla principal (`/`) muestra un resumen de tu preparación al momento.

### Tarjetas de estadísticas

Al abrir la aplicación verás 4 tarjetas numéricas en la parte superior:

| Tarjeta | Significado |
|---|---|
| **Temas Totales** | Número total de temas en el temario |
| **Pendientes** | Temas que aún no has empezado a estudiar |
| **En Progreso** | Temas que estás estudiando actualmente |
| **Dominados** | Temas que ya dominas completamente |

### Progreso global

Debajo de las tarjetas aparece la barra de **Progreso Global** expresada en porcentaje. Se calcula como:

```
Progreso global = (Temas dominados / Temas totales) × 100
```

### Progreso por materia

La sección inferior muestra el avance individual de cada materia con:
- Una barra de progreso de color (el color corresponde a la materia).
- El porcentaje completado.
- Conteo de temas en cada estado: pendientes, en progreso, dominados.

---

## 4. Materias

La sección **Materias** (`/materias`) presenta una cuadrícula de tarjetas, una por materia. Cada tarjeta muestra:

- Nombre de la materia con su color identificador (borde superior).
- Barra de progreso de la materia.
- Número total de temas y porcentaje completado.

### Acciones disponibles en cada tarjeta

| Botón | Acción |
|---|---|
| **📖 Ver temas →** | Abre el detalle de la materia con la lista completa de temas |
| **🃏 Flashcards** | Accede directamente a las flashcards de esa materia |

---

## 5. Detalle de materia y temas

Al entrar a una materia (`/materias/{id}`) verás la lista de todos sus temas en una tabla con las columnas:

- **Tema** — Nombre del tema.
- **Estado** — Badge de color indicando el estado actual.
- **Acciones** — Botones para cambiar el estado directamente desde la tabla.

Haz clic en cualquier fila de la tabla para abrir el **panel de detalle** de ese tema en la parte inferior de la página.

### 5.1 Cambiar estado de un tema

Cada fila de la tabla tiene tres botones de acción rápida:

| Botón | Estado resultante | Color del badge |
|---|---|---|
| **Pendiente** | El tema no ha sido estudiado | Gris |
| **En Progreso** | Estás estudiando el tema | Naranja / amarillo |
| **Dominado** | Ya dominas el tema completamente | Verde |

> **Tip:** Cambia el estado directamente desde la tabla sin necesidad de abrir el detalle. El cambio se guarda automáticamente.

### 5.2 Apuntes del tema

Al hacer clic en una fila, se abre un panel debajo de la tabla con los apuntes del tema. Puedes:

1. Escribir o editar libremente en el área de texto.
2. Presionar **"Guardar Apuntes"** para guardar los cambios.

Los apuntes se guardan en la base de datos y estarán disponibles cada vez que vuelvas al tema.

### 5.3 Material de estudio

En el mismo panel de detalle, después de los apuntes, está la sección **🔗 Material de Estudio**. Aquí puedes registrar recursos externos relacionados al tema (videos de YouTube, PDFs, artículos, etc.).

**Agregar un recurso:**

1. Haz clic en el botón **+ Agregar**.
2. Llena el formulario:
   - **Título** *(requerido)* — Nombre descriptivo del recurso.
   - **URL** *(opcional)* — Enlace web al recurso.
   - **Tipo** — Selecciona entre: `Video`, `PDF`, `Artículo`, `Apunte`, `Otro`.
3. Presiona **"Agregar"** para guardarlo.

**Gestionar recursos existentes:**

Cada recurso en la lista muestra:
- Ícono del tipo, nombre y enlace (si tiene URL).
- Botón **⌄** para expandir y editar las notas del recurso (puedes escribir un resumen o apuntes del material).
- Botón **🗑** para eliminar el recurso.

### 5.4 Preguntas del tema

Desde el detalle de la materia también puedes agregar preguntas de práctica para el simulador. Las preguntas que agregues aquí estarán disponibles en el **Simulador de Examen** para práctica aleatoria.

---

## 6. Flashcards

Las flashcards (`/materias/{id}/flashcards`) son tarjetas digitales de doble cara, ideales para memorizar conceptos, definiciones y fórmulas. Cada materia tiene su propio mazo de tarjetas.

### 6.1 Ver y voltear tarjetas

Las tarjetas se muestran en una cuadrícula. Cada tarjeta tiene:
- **FRENTE** — La pregunta, concepto o fórmula.
- **REVERSO** — La respuesta, definición o desarrollo.

**Para voltear una tarjeta:** Haz clic en ella. El lado visible cambia entre frente y reverso.

Si la tarjeta tiene etiquetas, aparecen en la parte inferior como chips de color.

Si el reverso contiene una gráfica, se renderizará automáticamente al voltear la tarjeta.

### 6.2 Crear una tarjeta nueva

1. Haz clic en **"+ Nueva Tarjeta"** (botón superior derecho).
2. Se abrirá el panel de edición con dos campos:
   - **Frente** — Editor de texto enriquecido. Escribe la pregunta o concepto. Soporta negrita, listas, imágenes y fórmulas LaTeX.
   - **Reverso** — Editor de texto enriquecido. Escribe la respuesta o explicación.
3. *(Opcional)* Escribe **etiquetas** separadas por coma para organizar las tarjetas (ej: `álgebra, funciones, límites`).
4. Presiona **"Guardar Tarjeta"**.

**Para editar una tarjeta existente:** Haz clic en el ícono de lápiz ✏️ en la esquina de la tarjeta.  
**Para eliminar una tarjeta:** Haz clic en el ícono de basura 🗑️.

### 6.3 Fórmulas LaTeX

Puedes escribir fórmulas matemáticas en el frente o el reverso usando la sintaxis LaTeX:

| Sintaxis | Resultado |
|---|---|
| `$x^2 + y^2 = r^2$` | Ecuación de círculo en línea |
| `$\frac{a}{b}$` | Fracción |
| `$\sqrt{x}$` | Raíz cuadrada |
| `$\int_a^b f(x)dx$` | Integral definida |
| `$\lim_{x \to 0} \frac{\sin x}{x}$` | Límite |

Las fórmulas se renderizan automáticamente al visualizar la tarjeta.

> **Hint visible en el editor:** "Usa `$fórmula$` para LaTeX (ej: `$x^2 + y^2 = r^2$`)"

### 6.4 Agregar gráfica a una tarjeta

Puedes incluir una gráfica interactiva en el **reverso** de una tarjeta:

1. En el editor, haz clic en **"📊 Agregar Gráfica al Reverso"**.
2. Aparecerá el panel de configuración de gráfica:
   - **Tipo de gráfica** — Selecciona: `Bar`, `Line`, `Area`, `Pie` o `Donut`.
   - **Título** *(opcional)* — Título descriptivo de la gráfica.
   - **Nombre de la serie** *(opcional)* — Ej: `Frecuencia`, `Temperatura`.
   - **Categorías** — Valores del eje X, separados por coma. Ej: `Ene, Feb, Mar`.
   - **Valores numéricos** — Valores del eje Y, separados por coma. Ej: `10, 25, 15`.
3. La gráfica se guardará junto con la tarjeta y se mostrará al ver el reverso.

Para **quitar la gráfica** de una tarjeta, haz clic nuevamente en "📊 Quitar Gráfica".

---

## 7. Simulador de Examen

El simulador (`/simulador`) genera un cuestionario con reactivos de opción múltiple tomados aleatoriamente del banco de preguntas.

### Configurar y empezar

1. En la pantalla inicial del simulador, selecciona el **número de preguntas** (entre 5 y 50).
2. Haz clic en **"Iniciar Simulacro"**.

> Si no hay preguntas registradas en la base de datos, aparecerá un mensaje indicándolo.

### Durante el simulacro

Para cada pregunta:
1. Lee el enunciado y las opciones (A, B, C, D y opcionalmente E).
2. Haz clic en la opción que consideres correcta.
3. La respuesta se evalúa de inmediato:
   - ✅ Verde = respuesta correcta.
   - ❌ Rojo = respuesta incorrecta (se resalta también la opción correcta en verde).
4. Si la pregunta tiene explicación registrada, se mostrará debajo.
5. Presiona **"Siguiente →"** para avanzar a la siguiente pregunta.
6. En la última pregunta el botón cambia a **"Ver Resultados"**.

### Resultados

Al terminar el simulacro, verás:
- Número de respuestas correctas sobre el total. Ej: `7 / 10 correctas`.
- Porcentaje de aciertos. Ej: `70.0%`.
- Botón **"Nuevo Simulacro"** para reiniciar con nuevas preguntas aleatorias.

Una barra de progreso en la parte superior indica en qué pregunta vas durante el simulacro.

---

## 8. Planner de Estudio

El Planner (`/calendario`) genera automáticamente un **plan de estudio distribuido** basado en el número de temas pendientes de cada materia y el tiempo disponible que definas.

### 8.1 Configurar el plan

Antes de generar el plan, ajusta los parámetros en el formulario de configuración:

| Campo | Descripción | Default |
|---|---|---|
| **Fecha de inicio** | Día en que empiezas a estudiar | Hoy |
| **Fecha de término** | Último día del plan | Hoy + 3 meses |
| **Horas de estudio por día** | Cuántas horas dedicarás diariamente (1–12) | 3 |
| **Días de repaso final** | Días al final del plan dedicados a repasar todas las materias (3–14) | 7 |
| **Solo días hábiles** | Excluir sábados y domingos del plan | Activado |

Haz clic en **"Generar Plan"** para calcular la distribución.

**El algoritmo asigna días a cada materia** proporcionalmente al número de temas pendientes. Las materias con más temas pendientes reciben más días.

### 8.2 Vista Cronograma (Gantt)

La vista **Cronograma** muestra un diagrama de Gantt con una fila por materia. Cada fila contiene:

- Nombre de la materia con su punto de color.
- **Temas** pendientes asignados.
- **Días** de estudio asignados.
- **Horas** totales para esa materia.
- **Fechas** de inicio y fin.
- **Barra visual** proporcional al tiempo asignado dentro del periodo total.

Una línea vertical roja indica el **día de hoy** en el cronograma.

> Puedes hacer scroll horizontal en la tabla si el periodo abarca muchos meses.

### 8.3 Vista Tablero

La vista **Tablero** presenta las materias como columnas tipo Kanban. Cada columna muestra:

- Encabezado con nombre, color y número de semanas.
- Chips con totales: días, horas y temas.
- Fechas de inicio y fin de la materia.
- **Tarjetas semanales** indicando por cada semana: fechas, horas estimadas y número aproximado de temas a cubrir.

### 8.4 Vista Calendario

La vista **Calendario** muestra el plan día a día, organizado por semanas. El plan se divide en dos fases:

| Fase | Descripción |
|---|---|
| **Aprendizaje** | Semanas dedicadas a ver material nuevo, alternando materias analíticas (Matemáticas, Física, Química, Biología) con humanísticas (Español, Literatura, Historia, Geografía) |
| **Repaso Final** | Últimas semanas del plan dedicadas a repasar todas las materias en rotación |

**Leyenda del calendario:**
- Chip sin borde especial → **Material nuevo** (fase Aprendizaje).
- Chip con borde punteado → **Repaso**.
- Semanas de repaso final tienen un fondo diferenciado con la etiqueta "Repaso Final".
- El **día de hoy** aparece resaltado en el calendario.

Cada día muestra una o más **pastillas de color** (una por materia asignada ese día) con el nombre de la materia y las horas de estudio para ese bloque.

---

## 9. Estados de avance

Los temas tienen tres estados que reflejan tu nivel de dominio:

| Estado | Color | Significado |
|---|---|---|
| **Pendiente** | Gris | Aún no has estudiado este tema |
| **En Progreso** | Naranja | Estás estudiando el tema pero no lo dominas todavía |
| **Dominado** | Verde | Entiendes y puedes resolver reactivos de este tema |

El **Planner de Estudio** toma en cuenta los temas en estado **Pendiente** y **En Progreso** para calcular la distribución. Los temas **Dominados** ya no se incluyen en el plan.

---

## 10. Flujo de trabajo recomendado

Sigue este orden para sacar el máximo provecho de la herramienta:

### Al inicio de tu preparación

1. Entra a **Materias** y revisa cuántos temas tiene cada materia.
2. Ve al **Planner** y genera un plan inicial con tus fechas de estudio disponibles.
3. Usa el Cronograma para tener una visión clara de en qué semanas estudiarás cada materia.

### Mientras estudias un tema

1. Abre la materia correspondiente en **Materias**.
2. Cambia el estado del tema a **En Progreso** cuando lo empieces.
3. Toma **apuntes** en el panel de detalle del tema.
4. Agrega los **recursos** que uses (videos, PDFs, apuntes digitales) con sus URLs.
5. Si la materia lo requiere, crea **flashcards** para conceptos o fórmulas clave.
6. Cuando termines de estudiar el tema y puedas resolver reactivos de él, cámbialo a **Dominado**.

### Para repasar

1. Usa las **flashcards** para repasar conceptos y fórmulas rápidamente.
2. Practica con el **Simulador de Examen** para identificar tus áreas débiles.
3. Consulta el **Dashboard** regularmente para monitorear tu avance general.

### Antes del examen

1. Genera un nuevo plan en el **Planner** con las semanas restantes para optimizar el repaso final.
2. Usa el Simulador con 30–50 preguntas para simular las condiciones reales del examen.
3. Repasa las flashcards de las materias con menor porcentaje de avance.

---

*Academic Planner — Preparación UNAM Área 2*
