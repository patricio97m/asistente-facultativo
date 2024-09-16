# Asistente facultativo

## Descripción
 En este proyecto se busca crear una inteligencia artificial capaz de ayudar a usuarios de los sistemas Miel e Intraconsulta, de la facultad Nacional de La Matanza. El objetivo de dicha IA es orientar a los alumnos para que puedan elegir materias que cursar de la forma más eficiente posible, prediciendo notas y horarios de estudio.

## Objetivo:
El objetivo de este trabajo de investigación es comprender y gestionar la
Inteligencia Artificial (IA), aplicándola en un proyecto llamado “Asistente
Facultativo”. Este mismo, busca predecir si un alumno puede promocionar una
materia basándose en diferentes factores como:
- El promedio de promoción del alumno
- El promedio de promoción de otros alumnos
- El promedio de promoción de la materia
- La cantidad de materias elegidas por el alumno
  
Además, se consideraron aspectos claves como:

- El horario en el que alumno desea dormir
- El horario de cursada (En caso de cursar o no)
- El horario de trabajo (En caso de trabajar o no)
La IA se encargará de predecir cuanto tiempo deberá dedicar el alumno a una
materia cada día para lograr la promoción.

## Modelos y tecnologías utilizadas
- **LightGbmRegression**: Componente que entrena un modelo de regresión de árbol de decisión con un aumento de gradiente utilizando LightGBM (Light Gradient Boosting Machine). LightGBM es un algoritmo de aprendizaje automático de código abierto
- **LightGbmMulticlass**: Es un modelo de clasificación que utiliza la misma técnica de LightGBM para predecir clases discretas en lugar de valores continuos. Es ideal para tareas donde el objetivo es clasificar datos en múltiples categorías.
