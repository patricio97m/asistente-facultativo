let contadorFilas = 1;

function agregarEvento(dia, horaInicio, horaFin, actividad) {
    let diaIndex = getDiaIndex(dia.toLowerCase());
    let tbody = document.getElementById('calendario');

    // Buscar si hay una fila existente con una celda vacía para el día especificado
    let filas = tbody.getElementsByTagName('tr');
    let foundCell = null;
    for (let i = 0; i < filas.length; i++) {
        let cell = filas[i].children[diaIndex];
        if (cell.innerHTML === '') {
            foundCell = cell;
            break;
        }
    }

    // Si no se encontró una celda vacía, agregar una nueva fila
    if (!foundCell) {
        let newRow = document.createElement('tr');
        newRow.id = `horario${contadorFilas++}`;
        for (let i = 0; i < 7; i++) {
            let newCell = document.createElement('td');
            newCell.id = getDiaString(i) + contadorFilas;
            newRow.appendChild(newCell);
            newCell.classList.add('bg-success', 'rounded', 'text-white', 'text-center', 'fw-bold', 'p-2', 'border'); 
        }
        tbody.appendChild(newRow);
        foundCell = newRow.children[diaIndex];
    }

    // Crear el evento y agregarlo a la celda
    let evento = document.createElement('p');
    evento.className = 'evento';
    evento.innerHTML = `${horaInicio}-${horaFin} <br>${actividad}`;
    foundCell.appendChild(evento);
}

function getDiaIndex(dia) {
    switch (dia) {
        case 'lunes':
            return 0;
        case 'martes':
            return 1;
        case 'miercoles':
            return 2;
        case 'jueves':
            return 3;
        case 'viernes':
            return 4;
        case 'sabado':
            return 5;
        case 'domingo':
            return 6;
        default:
            return -1;
    }
}

function getDiaString(index) {
    switch (index) {
        case 0:
            return 'lunes';
        case 1:
            return 'martes';
        case 2:
            return 'miercoles';
        case 3:
            return 'jueves';
        case 4:
            return 'viernes';
        case 5:
            return 'sabado';
        case 6:
            return 'domingo';
        default:
            return '';
    }
}

// Ejemplo de uso
//let eventos = [
//    { Dia: 'Lunes', Inicio: '8:00', Fin: '12:00', Descripcion: 'Programación Web 1' },
//    { Dia: 'Lunes', Inicio: '00:00', Fin: '00:00', Descripcion: 'Duermo' }
//];

//eventos.forEach((e) => {
//    console.log(e.Dia, e.Inicio, e.Fin, e.Descripcion);
//    agregarEvento(e.Dia, e.Inicio, e.Fin, e.Descripcion);
//});
